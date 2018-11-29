//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       DatabaseHelper
//创建时间:     2015/10/15 14:36:27
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Const;
using EyeSight.Model.Daily;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Helper
{
    public class DatabaseHelper : ClassBase<DatabaseHelper>
    {
        public DatabaseHelper() : base() { }
        public async Task<List<ModelPropertyBase>> QueryCollections(bool isDownloadDatabase = false)
        {
            List<ModelPropertyBase> ln = new List<ModelPropertyBase>();

            var dataModelConnection = isDownloadDatabase ? DicStore.GetValueOrDefault<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_DOWNLOAD_DATABASE, null) : DicStore.GetValueOrDefault<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_FAVORITE_DATABASE, null);
            if (dataModelConnection != null)
            {

                var list = await dataModelConnection.QueryAsync<ModelPropertyBase>("select * from VideoInfo");
                foreach (var l in list)
                {
                    var p = await dataModelConnection.QueryAsync<Provider>("select * from Provider where parentId = " + l.id);
                    l.provider = p.FirstOrDefault();

                    var c = await dataModelConnection.QueryAsync<Consumption>("select * from Consumption where parentId = " + l.id);
                    l.consumption = c.FirstOrDefault();

                    var pi = await dataModelConnection.QueryAsync<Playinfo>("select * from Playinfo where parentId = " + l.id);
                    l.playInfo = pi;

                    var tags = await dataModelConnection.QueryAsync<VideoTag>("select * from VideoTag where parentId = " + l.id);
                    l.tags = tags;
                }
                ln = list;
            }
            return ln;
        }

        public async Task<bool> InsertBySingle(ModelPropertyBase item, bool isDownloadDatabase = false)
        {
            var dataModelConnection = isDownloadDatabase ? DicStore.GetValueOrDefault<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_DOWNLOAD_DATABASE, null) : DicStore.GetValueOrDefault<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_FAVORITE_DATABASE, null);
            if (dataModelConnection != null)
            {
                if (item != null)
                {
                    if (await dataModelConnection.InsertAsync(item) == 0)
                    {
                        await dataModelConnection.UpdateAsync(item);
                    }

                    var provider = item.provider;
                    if (provider != null)
                    {
                        provider.parentId = item.id;

                        if (await dataModelConnection.InsertAsync(item.provider) == 0)
                        {
                            await dataModelConnection.UpdateAsync(item.provider);
                        }
                    }

                    var consumption = item.consumption;
                    if (consumption != null)
                    {
                        consumption.parentId = item.id;

                        if(await dataModelConnection.InsertAsync(item.consumption) == 0)
                        {
                            await dataModelConnection.UpdateAsync(item.consumption);
                        }
                    }

                    var playInfo = item.playInfo;
                    if (playInfo != null)
                    {
                        playInfo.ForEach(A =>
                        {
                            A.parentId = item.id;
                        });

                        if (await dataModelConnection.InsertAllAsync(item.playInfo) == 0)
                        {
                            await dataModelConnection.UpdateAllAsync(item.playInfo);
                        }
                    }

                    var videoTags = item.tags;
                    if (videoTags != null)
                    {
                        videoTags.ForEach(A =>
                        {
                            A.parentId = item.id;
                        });

                        if (await dataModelConnection.InsertAllAsync(item.tags) == 0)
                        {
                            await dataModelConnection.UpdateAllAsync(item.tags);
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteBySingle(ModelPropertyBase item, bool isDownloadDatabase = false)
        {
            var dataModelConnection = isDownloadDatabase ? DicStore.GetValueOrDefault<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_DOWNLOAD_DATABASE, null) : DicStore.GetValueOrDefault<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_FAVORITE_DATABASE, null);
            if (dataModelConnection != null)
            {
                if (item != null)
                {
                    item.playInfo.ForEach(async A =>
                    {
                        await dataModelConnection.DeleteAsync(A);
                    });

                    item.tags.ForEach(async A =>
                    {
                        await dataModelConnection.DeleteAsync(A);
                    });

                    await dataModelConnection.DeleteAsync(item.provider);

                    await dataModelConnection.DeleteAsync(item.consumption);

                    await dataModelConnection.DeleteAsync(item);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
