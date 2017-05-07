﻿using System;
using BiraFit.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BiraFit.ViewModel;
using BiraFit.Controllers.Helpers;

namespace BiraFit.Controllers
{
    public class NachrichtController : BaseController
    {
        // GET: Nachricht
        public ActionResult Index()
        {
            List<Konversation> konversationList;
            if (IsSportler())
            {
                Sportler sportler = AuthentificationHelper.AuthenticateSportler(User, Context);
                var konversationen = from b in Context.Konversation
                    where b.Sportler_Id == sportler.Id
                    select b;
                konversationList = konversationen.ToList();
            }
            else
            {
                PersonalTrainer trainer = AuthentificationHelper.AuthenticatePersonalTrainer(User, Context);
                var konversationen = from b in Context.Konversation
                    where b.PersonalTrainer_Id == trainer.Id
                    select b;
                konversationList = konversationen.ToList();
            }

            List<string> lastMessages = new List<string>();
            List<string> profileImages = new List<string>();
            foreach (var konversation in konversationList)
            {
                lastMessages.Add(GetLastMessage(konversation.Id));

                if (IsSportler())
                {
                    string trainerId = GetTrainerAspNetUserId(konversation.PersonalTrainer_Id);
                    var profileImage = Context.Users.Single(i => i.Id == trainerId).ProfilBild ??
                                       "standardprofilbild.jpg";
                    profileImages.Add(profileImage);
                }
                else
                {
                    string sportlerId = GetSportlerAspNetUserId(konversation.Sportler_Id);
                    var profileImage = Context.Users.Single(i => i.Id == sportlerId).ProfilBild ??
                                       "standardprofilbild.jpg";
                    profileImages.Add(profileImage);
                }
            }

            return View(new NachrichtViewModel()
            {
                Konversationen = konversationList,
                LastMessages = lastMessages,
                ProfileImages = profileImages
            });
        }

        // GET: Nachricht/Chat/<id>
        public ActionResult Chat(int id)
        {
            if (!CheckPermission(id))
            {
                return RedirectToAction("Index", "Home");
            }

            var chat = from k in Context.Konversation
                where k.Id == id
                from m in k.Nachrichten
                orderby m.Datum
                select m;

            List<Nachricht> chatList = chat.ToList();

            return View(new ChatViewModel
            {
                Nachrichten = chatList,
                KonversationId = id,
                Id = User.Identity.GetUserId()
            });
        }

        [HttpPost]
        public ActionResult SendMessage(ChatViewModel message)
        {
            var konversation = Context.Konversation.First(i => i.Id == message.KonversationId);
            string empfaengerId = User.Identity.GetUserId() == GetTrainerAspNetUserId(konversation.PersonalTrainer_Id)
                ? GetSportlerAspNetUserId(konversation.Sportler_Id)
                : GetTrainerAspNetUserId(konversation.PersonalTrainer_Id);
            string query =
                $"INSERT INTO Nachricht (Text,Sender_Id,Empfaenger_Id,Datum,Konversation_Id,Konversation_Id1) VALUES ('{message.Nachricht}','{User.Identity.GetUserId()}','{empfaengerId}',CONVERT(datetime, '{DateTime.Now}', 104),{message.KonversationId},{message.KonversationId})";
            Context.Database.ExecuteSqlCommand(query);
            return RedirectToAction("Chat/" + message.KonversationId, "Nachricht");
        }

        public bool CheckPermission(int konversationId)
        {
            if (!IsLoggedIn())
            {
                return false;
            }

            var konversation = GetKonversation(konversationId);

            if (IsSportler())
            {
                return konversation.Sportler_Id == GetUserIdbyAspNetUserId(User.Identity.GetUserId());
            }

            return konversation.PersonalTrainer_Id == GetUserIdbyAspNetUserId(User.Identity.GetUserId());
        }

        public Konversation GetKonversation(int konversationId)
        {
            return Context.Konversation.Single(k => k.Id == konversationId);
        }

        public string GetLastMessage(int konversationId)
        {
            var nachrichten = Context.Nachricht.Where(n => n.Konversation_Id == konversationId).OrderBy(m => m.Datum);
            string lastMessage = "";
            foreach (var nachricht in nachrichten)
            {
                lastMessage = nachricht.Text;
            }
            return lastMessage;
        }
    }
}