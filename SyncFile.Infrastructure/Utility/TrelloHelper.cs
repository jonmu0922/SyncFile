using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SyncFile.Infrastructure.Utility
{
    public class TrelloHelper
    {
        /// <summary>
        /// id 轉換建立時間
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DateTime IdToDatetime(string id)
        {
            // https://help.trello.com/article/759-getting-the-time-a-card-or-board-was-created

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddSeconds(Convert.ToInt32(id.Substring(0, 8), 16));

            // 台灣時間 +8
            return epoch.AddHours(8);
        }

        public static string GetCardList(string key, string token, string list)
        {
            string httpresult = HttpHelper.HttpGet(
                string.Format(
                    "https://api.trello.com/1/lists/{2}/cards?key={0}&token={1}",
                    key, token, list
                )
            );

            return httpresult;
        }

        public static string GetCardAttachmentList(string key, string token, string card)
        {
            string httpresult = HttpHelper.HttpGet(
                string.Format(
                    "https://api.trello.com/1/cards/{2}/attachments?key={0}&token={1}",
                    key, token, card
                )
            );

            return httpresult;
        }

        public static string GetCard(string key, string token, string card)
        {
            string httpresult = HttpHelper.HttpGet(
                string.Format(
                    "https://api.trello.com/1/cards/{2}?key={0}&token={1}",
                    key, token, card
                )
            );

            return httpresult;
        }

        public static string CreateCard(string key, string token, string list, string name, string desc = "")
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                ["key"] = key,
                ["token"] = token,
                ["idList"] = list,
                ["name"] = name,
                ["desc"] = desc,
            };

            string httpresult = HttpHelper.HttpPost(
                "https://api.trello.com/1/cards",
                para
            );

            return httpresult;
        }

        public static string DeleteCard(string key, string token, string id)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                ["key"] = key,
                ["token"] = token
            };

            string httpresult = HttpHelper.HttpDelete(
                string.Format("https://api.trello.com/1/cards/{0}",id),
                para
            );

            return httpresult;
        }

        /// <summary>
        /// 建立附件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string CreateAttachment(string key, string token, string id, 
            string name, byte[] file)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                ["key"] = key,
                ["token"] = token
                //["name"] = name              
            };

            string httpresult = HttpHelper.HttpPost(
                string.Format("https://api.trello.com/1/cards/{0}/attachments", id),
                para,
                name,
                file
            );

            return httpresult;
        }

        /// <summary>
        /// 刪除附件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <param name="card"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static string DeleteAttachment(string key, string token, string card, string attachment)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                ["key"] = key,
                ["token"] = token
            };

            string httpresult = HttpHelper.HttpDelete(
                string.Format("https://api.trello.com/1/cards/{0}/attachments/{1}", card, attachment),
                para
            );

            return httpresult;
        }

        /// <summary>
        /// 下載附件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetAttachment(string path)
        {
            return HttpHelper.HttpGetByte(path);
        }
    }
}
