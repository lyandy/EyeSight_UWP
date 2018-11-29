//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.Selector

//类名称:       AuthorDetailListViewHeaderDataTemplateSelector

//创建时间:     2016/7/12 16:48:51

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using EyeSight.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EyeSight.Selector
{
    public abstract class MyDataTemplateSelector : ContentControl
    {
        //根据Content的属性，返回所需的DataTemplate
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            //根据newContent的属性，选择对应的DataTemplate
            ContentTemplate = SelectTemplate(newContent, this);
        }
    }



    public class AuthorDetailListViewHeaderDataTemplateSelector : MyDataTemplateSelector
    {
        public DataTemplate　AuthorDetailHeaderNormalDataTemplate { get; set; }
        public DataTemplate AuthorDetailHeaderNullDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //var vm = item as NewsViewModel;
            if (AppEnvironment.IsPhone)
            {
                return AuthorDetailHeaderNormalDataTemplate;
            }
            else
            {
                return AuthorDetailHeaderNullDataTemplate;
            }
        }
    }
}
