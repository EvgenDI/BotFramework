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
    [LuisModel("", "")]



    public class RootDialog : LuisDialog<object>
    {
        private string cod;
        private string newNumberPhone;
        private int count;
        //getProced=0 - Email , getProced=1 - Phone
        private int getProced;
        private EntityRecommendation entityConteiner;
        private string numberPhone;
        Email clEmail = new Email(host,port,user,password,from);
        Postgres clPostgres = new Postgres("db connection string");
        SmsRu sms = new SmsRu("ApiKey");
        ThreadRandom random = new ThreadRandom();



        [LuisIntent("")]
        public async Task ProcessNone(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Простите, я вас не понимаю");
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
            PromptDialog.Confirm(context, Say, $"Редактировать электронную почту?");
        }



        [LuisIntent("Mail")]
        public async Task ProcessMail(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Введите адрес электронной почты");
            context.Wait(MessageReceived);
        }



        [LuisIntent("GetEmail")]
        public async Task ProcessGetEmail(IDialogContext context, LuisResult result)
        {
            if (result.TryFindEntity("builtin.email", out entityConteiner))
            {
                if (clEmail.Correct(entityConteiner.Entity))
                {
                    if (clPostgres.SearchEmail(entityConteiner) == true)
                    {
                        await SendCode(context);
                    }
                    else
                    {
                        await context.PostAsync("Указанный адрес электронной почты уже используется.Введите другой адрес электронной почты ");
                        context.Wait(MessageReceived);
                    }
                }
                else
                {
                    await context.PostAsync("Введен некорректный адрес электронной почты ");
                    context.Wait(MessageReceived);
                }
            }
        }



        [LuisIntent("Phone")]
        public async Task ProcessPhone(IDialogContext context, LuisResult result)
        {
            getProced = 1;
            await context.PostAsync("Введите номер мобильного телефона.Формат 8 ### ### ## ##");
            context.Wait(SendPhone);
            
        }



        private async Task SendPhone(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;
            numberPhone = msg.Text;
            if (msg.Text.ToLower() == "отмена")
            {
                await context.PostAsync("Отмена");
                context.Wait(MessageReceived);
            }
            else
            {
                if (sms.CorrectPhone(numberPhone) == true)
                {
                    newNumberPhone = sms.DeleteSymbol(numberPhone);
                    if (clPostgres.SearchPhone(newNumberPhone) == true)
                    {
                        await SendCodePhone(context, numberPhone);
                    }
                    else
                    {
                        await context.PostAsync("Указанный номер мобильного телефона уже используется. Введите другой номер мобильного телефона");
                        context.Wait(SendPhone);
                    }
                }
                else
                {
                    await context.PostAsync("Введен некорректный номер мобильного телефона.Повторите попытку");
                    context.Wait(SendPhone);
                }
            }
        }



        private async Task EditCod(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;
            if (msg.Text.ToLower() == "отмена")
            {
                await context.PostAsync("Отмена");
                context.Wait(MessageReceived);
            }
            else
            {
                if (msg.Text == cod)
                {
                    if (getProced == 0)
                    {
                        clPostgres.PostgreSql(entityConteiner);
                        await context.PostAsync("Адрес электронной почты успешно сохранен");
                        context.Wait(MessageReceived);
                    }
                    else
                    {
                        clPostgres.PostgreSql(newNumberPhone);
                        await context.PostAsync("Номер мобильного телефона успешно сохранен");
                        context.Wait(MessageReceived);
                    }
                }
                else
                {
                    count--;
                    await context.PostAsync($"Неправильный код, осталось попыток " + count);
                    if (getProced == 0)
                    { 
                        if (count == 0)
                        {
                            PromptDialog.Confirm(context, ErrorCode, $"Использовать другой адрес электронной почты?");
                        }
                        else
                        {
                            context.Wait(EditCod);
                        }
                    }
                    else
                    {
                        if (getProced == 1)
                        {
                            if (count == 0)
                            {
                                PromptDialog.Confirm(context, ErrorCode, $"Использовать другой номер мобильного телефона?");
                            }
                            else
                            {
                                context.Wait(EditCod);
                            }
                        }
                    }
                }
            }           
        }



        private async Task  ErrorCode(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            if (msg)
            {
                await context.PostAsync("Введите адрес электронной почты");
                context.Wait(MessageReceived);
            }
            else
            {
                PromptDialog.Confirm(context,RepeatCode, $"Выслать код подтверждения повторно?");
            }
        }



        private async Task ErrorCodePhone(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            if (msg)
            {
                await context.PostAsync("Введите номер мобильного телеффона");
                context.Wait(SendPhone);
            }
            else
            {
                PromptDialog.Confirm(context, RepeatCode, $"Выслать код подтверждения повторно?");
            }
        }



        private async Task RepeatCode(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            if (msg)
            {
                if (getProced == 0)
                {
                    await SendCode(context);
                }
                else
                {
                    await SendCodePhone(context, numberPhone);
                }
            }
            else
            {
                await context.PostAsync("Пока");
                context.Wait(MessageReceived);
            }
        }



        private async Task Say(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            if (msg)
            {                
                await context.PostAsync("Введите адрес электронной почты");
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Пока");
                context.Wait(MessageReceived);
            }
        }



        private async Task SendCode(IDialogContext context)
        {
            cod = random.GetRandom().Result.ToString();
            count = 3;
            getProced = 0;
            await clEmail.SendEmailAsync(entityConteiner, cod);
            await context.PostAsync("Сообщение с кодом потверждения отправлено,введите код подтверждения");
            context.Wait(EditCod);
        }



        private async Task SendCodePhone(IDialogContext context,string number)
        {
            cod = random.GetRandom().Result.ToString();
            count = 3;
            sms.Send(number, cod);
            await context.PostAsync("Сообщение с кодом потверждения отправлено,введите код подтверждения");
            context.Wait(EditCod);

        }
     }
 }