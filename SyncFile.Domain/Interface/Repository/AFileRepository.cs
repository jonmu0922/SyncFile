using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace SyncFile.Domain.Interface.Repository
{
    public abstract class AFileRepository
    {
        protected XDocument _xdoc = new XDocument();

        protected string GetSyncID()
        {
            return _xdoc.Element("sync").Element("id").Value;
        }

        /// <summary>
        /// 初始化設定檔
        /// </summary>
        protected void InitSyncSetting()
        {
            // 建立新的設定檔
            XElement root = new XElement("sync");

            XElement id = new XElement("id")
            {
                Value = Guid.NewGuid().ToString()
            };

            root.Add(id);
            _xdoc.Add(root);            
        }

        /// <summary>
        /// 取得最後一筆同步資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected DateTime? GetLastSyncRecord(string id)
        {
            if (_xdoc.Element("sync").Element("records") != null)
            {
                var record = _xdoc.Element("sync").Element("records")
                    .Elements("record")
                    .Where(o => o.Attribute("id").Value == id)
                    .OrderByDescending(o => o.Value)
                    .FirstOrDefault();

                if (record != null)
                    return DateTime.Parse(record.Value);
            }

            return null;
        }

        /// <summary>
        /// 新增一筆同步紀錄
        /// </summary>
        /// <param name="id"></param>
        protected void AddSyncRecord(string id)
        {
            XElement history = new XElement("record", new XAttribute("id", id))
            {
                Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };

            if (_xdoc.Element("sync").Element("records") == null)
                _xdoc.Element("sync").Add(new XElement("records"));

            _xdoc.Element("sync").Element("records").Add(history);
        }
    }
}
