using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Npgsql;
using System.Collections.Generic;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    [LuisModel("1c503cd1-0f49-4e00-a21b-61e508f8688e", "878007334e2c4ad184de582be1e043f4")]

    public class RootDialog : LuisDialog<object>
    {

        private string cod;
        private int count;
        private EntityRecommendation entityConteiner;

        [LuisIntent("")]
        public async Task ProcessNone(IDialogContext context, LuisResult result)
        {
            var massage = "Простите, я вас не понимаю";
            await context.PostAsync(massage);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Hello")]
        public async Task ProcessHello(IDialogContext context, LuisResult result)
        {
            var massages = new string[]
            {
                "Привет!",
                "Привет)",
                "Здравствуйте!",
                "Чем могу помочь?"
            };
            var massage = massages[(new Random()).Next(massages.Count() - 1)];
            await context.PostAsync(massage);
            PromptDialog.Confirm(context, SAY, $"Редактировать электронную почту?");
            
        }

      

        [LuisIntent("mail")]
        public async Task Processmail(IDialogContext context, LuisResult result)
        {
            var massage = "Введите адрес электронной почты";

            await context.PostAsync(massage);
            context.Wait(MessageReceived);
        }




        
        [LuisIntent("GetEmail")]
        public async Task ProcessGetEmail(IDialogContext context, LuisResult result)
        {
            var massage = "Введен некорректный адрес электронной почты ";
            
            

            if (result.TryFindEntity("builtin.email", out entityConteiner))
            {

                if (Email.correct(entityConteiner.Entity))
                {
                    if (Postgres.SearchEmail(entityConteiner) == true)
                    {
                        cod = ThreadRandom.getRandom().Result.ToString();
                        count = 3;
                        await Email.SendEmailAsync(entityConteiner, cod);
                        await context.PostAsync("Сообщение с кодом потверждения отправлено,введите код подтверждения");
                        context.Wait(Editcod);
                }
                else
                {
                    await context.PostAsync($"Указанный адрес электронной почты уже используется.Введите другой адрес электронной почты " + cod);
                    context.Wait(MessageReceived);
                }
            }
                else
                {
                    await context.PostAsync(massage);
                    context.Wait(MessageReceived);
                }
            }

           

        }

        private async Task Editcod(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            

            var msg = await argument;

            if (msg.Text.ToLower() == "отмена")
            {
                await context.PostAsync($"Отмена");
                context.Wait(MessageReceived);
            }
            else
            {
                if (msg.Text == cod)
                {
                    Postgres.PostgresSql(entityConteiner);
                    await context.PostAsync($"Адрес электронной почты успешно сохранен");
                    context.Wait(MessageReceived);
                }
                else
                {
                    count--;
                    await context.PostAsync($"Неправильный код, осталось попыток " + count );
                    if (count == 0)
                    {
                        PromptDialog.Confirm(context, ErrorCode, $"Использовать другой адрес электронной почты?");
                    }
                    else
                    {
                        context.Wait(Editcod);
                    }

                }
            }
            

        }

        private async Task  ErrorCode(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            if (msg)
            {

                var massage = "Введите адрес электронной почты";
                await context.PostAsync(massage);
                context.Wait(MessageReceived);
            }
            else
            {
                PromptDialog.Confirm(context,RepeatCode, $"Выслать код подтверждения повторно?");
            }
        }

        private async Task RepeatCode(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            if (msg)
            {
                count = 3;
                await Email.SendEmailAsync(entityConteiner, cod);
                var massage = "Сообщение с кодом потверждения отправлено,введите код подтверждения" + cod;
                await context.PostAsync(massage);
                context.Wait(Editcod);
            }
            else
            {
                var massage = "Пока";
                await context.PostAsync(massage);
                context.Wait(MessageReceived);
            }
        }

        private async Task SAY(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            if (msg)
            {
                
                var massage = "Введите адрес электронной почты";
                await context.PostAsync(massage);
                context.Wait(MessageReceived);
            }
            else
            {
                var massage = "Пока";
                await context.PostAsync(massage);
                context.Wait(MessageReceived);
            }
        }
       

    }
    }