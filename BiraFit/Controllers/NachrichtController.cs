﻿using System;
using BiraFit.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BiraFit.ViewModel;
using BiraFit.Controllers.Helpers;
using NUnit.Framework;

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
                var sportler = AuthentificationHelper.AuthenticateSportler(User, Context);
                var sportlerKonversationen = from b in Context.Konversation
                    where b.Sportler_Id == sportler.Id
                    select b;
                konversationList = sportlerKonversationen.ToList();
                return View(konversationList);
            }

            var personalTrainer = AuthentificationHelper.AuthenticatePersonalTrainer(User, Context);
            var trainerKonversationen = from b in Context.Konversation
                where b.PersonalTrainer_Id == personalTrainer.Id
                select b;
            konversationList = trainerKonversationen.ToList();
            return View(konversationList);

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

            return View(new ChatViewModel { Nachrichten = chatList, KonversationId = id, Id = User.Identity.GetUserId() });
        }

        [HttpPost]
        public ActionResult SendMessage(ChatViewModel message)
        {
            var konversation = Context.Konversation.First(i => i.Id == message.KonversationId);
            string empfaengerId = User.Identity.GetUserId() == GetTrainerAspNetUserId(konversation.PersonalTrainer_Id) ? GetSportlerAspNetUserId(konversation.Sportler_Id) : GetTrainerAspNetUserId(konversation.PersonalTrainer_Id);

            /* funktioniert nicht ganz wegen Konversation_Id1
            Nachricht nachricht = new Nachricht()
            {
                Datum = DateTime.Now,
                Empfaenger_Id = empfaengerId,
                Sender_Id = User.Identity.GetUserId(),
                Text = message.Nachricht,
                Konversation_Id = message.KonversationId,
                
            };
            Context.Nachricht.Add(nachricht);
            Context.SaveChanges();
            */
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
    }
}