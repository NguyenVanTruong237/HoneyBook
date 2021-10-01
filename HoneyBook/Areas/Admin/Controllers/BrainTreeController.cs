﻿using Braintree;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrainTreeController : Controller
    {
        public IBrainTreeGate _brain { get; set; }
        public BrainTreeController(IBrainTreeGate brain)
        {
            _brain = brain;
        }
        public IActionResult Index()
        {
            var gateway = _brain.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IFormCollection collection)
        {
            Random rnd = new Random();
            string nonceFromtheClient = collection["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = rnd.Next(1, 10),
                PaymentMethodNonce = nonceFromtheClient,
                OrderId = "45",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var gateway = _brain.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.Target.ProcessorResponseText == "Approved")
            {
                TempData["Success"] = "Transaction was successfully Transaction ID"
                    + result.Target.Id + ", Amount Charged: $" + result.Target.Amount;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
