using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Models.Message;
using BookingAds.Common.Repository.Employee;
using BookingAds.Common.Repository.Message.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Models.Messenger;
using BookingAds.Modules;
using BookingAds.Services;
using BookingAds.Services.Abstractions;

namespace BookingAds.Controllers
{
    [RoleFilter(Roles = RoleConstant.EMPLOYEE)]
    public class MessengerController : Controller
    {
        #region Repository
        private readonly IMessageRepository _messageRepo = new MessageRepository();
        #endregion
        #region Service
        private readonly ISendMessageService _sendMessageService = new ChatGPTSendMessageApiService();
        #endregion
        #region Constant
        private const string ControllerName = "Messenger";
        private const string ActionIndex = "Index";
        private const string ActionChatHistory = "ChatHistory";
        private const string ActionChat = "Chat";
        private const string ActionRead = "Read";
        private const string ChatGPT = nameof(ChatGPT);
        #endregion
        #region Action

        // GET: Messenger
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            var model = new ViewChat()
            {
                Admins = _messageRepo.GetAdmins(string.Empty),
            };

            return View(model);
        }

        // POST: Messenger/ChatHistory
        [HttpPost]
        [ActionName(ActionChatHistory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult ChatHistory(string userName = "", int limit = 5)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }

            var myUserName = ConvertUtils<Account>.Deserialize(User.Identity.Name).UserName;
            var model = new ViewChatHistory()
            {
                Messages = _messageRepo.GetMessages(myUserName, userName, limit),
            };

            return View(model);
        }

        // POST: Messenger/Chat
        [HttpPost]
        [ActionName(ActionChat)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Chat(ViewCreateMessage dataDto)
        {
            if (dataDto == null)
            {
                return Json(ConstantsMessenger.ErrorMessenger, JsonRequestBehavior.AllowGet);
            }

            var newMessageID = _messageRepo.CreateMessage(dataDto);
            if (newMessageID == 0)
            {
                return Json(ConstantsMessenger.FailCreateMessenger, JsonRequestBehavior.AllowGet);
            }

            // when chat with Chat GPT
            if (dataDto.ReceiverID == ChatGPT)
            {
                var answerGPT = _sendMessageService.SendMessage(dataDto.Content);
                if (string.IsNullOrEmpty(answerGPT))
                {
                    return Json(ConstantsMessenger.FailCreateMessenger, JsonRequestBehavior.AllowGet);
                }

                // create new message with senderID is Chat GPT and receiverID is current userName
                var newDataDto = new ViewCreateMessage()
                {
                    SenderID = dataDto.ReceiverID,
                    ReceiverID = dataDto.SenderID,
                    Content = answerGPT,
                    CreatedTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"),
                };

                var messageIDCreated = _messageRepo.CreateMessage(newDataDto);
                if (messageIDCreated == 0)
                {
                    return Json(ConstantsMessenger.FailCreateMessenger, JsonRequestBehavior.AllowGet);
                }

                return Json(newDataDto, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        // POST: Messenger/Read
        [HttpPost]
        [ActionName(ActionRead)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Read(ViewReadMessage dataDto)
        {
            if (dataDto == null)
            {
                return Json(ConstantsMessenger.ErrorMessenger, JsonRequestBehavior.AllowGet);
            }

            var isCreated = _messageRepo.ReadMessage(dataDto);
            if (!isCreated)
            {
                return Json(ConstantsMessenger.FailReadMessenger, JsonRequestBehavior.AllowGet);
            }

            return Json(ConstantsMessenger.ReadSuccessMessenger, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}