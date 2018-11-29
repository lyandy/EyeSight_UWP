//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       TileHelper
//创建时间:     2015/11/23 19:41:10
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Encrypt;
using EyeSight.Model.Daily;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace EyeSight.Helper
{
    public class TileHelper : ClassBase<TileHelper>
    {
        public TileHelper() : base() { }

        TileUpdater updater = null;

        XmlDocument doc = null;

        public async void UpdateTiles(List<Videolist> collection)
        {
            if (updater == null)
            {
                updater = TileUpdateManager.CreateTileUpdaterForApplication();
            }
            updater.EnableNotificationQueueForWide310x150(true);
            updater.EnableNotificationQueueForSquare150x150(true);
            updater.EnableNotificationQueueForSquare310x310(true);
            updater.EnableNotificationQueue(true);

            foreach (var model in collection)
            {
                try
                {
                    var pcFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.PCSplashScreenImageCacheRelativePath, CreationCollisionOption.OpenIfExists);
                    var fileName = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(model.coverForDetail);
                    var file = await pcFolder.TryGetItemAsync(fileName);

                    string xmlStr = null;

                    if (file != null)
                    {
                        xmlStr =
                            "<tile version='3'>"
                        + "<visual>"
                        + "<binding template='TileMedium' branding='name'>"
                        + "<image placement='peek' src='" + file.Path + "'/>"
                        + "<text hint-wrap='true'>" + model.title + "</text>"
                        + "</binding>"

                        + "<binding template='TileWide' branding='name'>"
                        + "<image placement='peek' src='" + file.Path + "'/>"
                        + "<text hint-wrap='true'>" + model.title + "</text>"
                        + "<text hint-style='captionsubtle' hint-wrap='true'>" + model.description + "</text>"
                        + "</binding>"

                        + "<binding template='TileLarge' branding='name'>"
                        + "<image placement='peek' src='" + file.Path + "'/>"
                        + "<text hint-wrap='true'>" + model.title + "</text>"
                        + "<text hint-style='captionsubtle' hint-wrap='true'>" + model.description + "</text>"
                        + "</binding>"
                        + "</visual>"
                        + "</tile>";
                    }
                    else
                    {
                        xmlStr =
                            "<tile version='3'>"
                        + "<visual>"
                        + "<binding template='TileMedium' branding='name'>"
                        + "<text hint-wrap='true'>" + model.title + "</text>"
                        + "</binding>"

                        + "<binding template='TileWide' branding='name'>"
                        + "<text hint-wrap='true'>" + model.title + "</text>"
                        + "<text hint-style='captionsubtle' hint-wrap='true'>" + model.description + "</text>"
                        + "</binding>"

                        + "<binding template='TileLarge' branding='name'>"
                        + "<text hint-wrap='true'>" + model.title + "</text>"
                        + "<text hint-style='captionsubtle' hint-wrap='true'>" + model.description + "</text>"
                        + "</binding>"
                        + "</visual>"
                        + "</tile>";
                    }

                    if (doc == null)
                    {
                        doc = new XmlDocument();
                    }

                    doc.LoadXml(xmlStr.Replace("&", "-"));

                    updater.Update(new TileNotification(doc));
                }
                catch(Exception ex)
                {
                    string s = ex.ToString();
                }
            }
        }

        public void CloseTiles()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }
    }
}
