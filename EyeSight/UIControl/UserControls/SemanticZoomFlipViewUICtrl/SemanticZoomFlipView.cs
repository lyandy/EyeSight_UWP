//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.SemanticZoomFlipViewUICtrl
//类名称:       SemanticZoomFlipView
//创建时间:     2015/9/21 星期一 21:19:18
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EyeSight.UIControl.UserControls.SemanticZoomFlipViewUICtrl
{
    public sealed class SemanticZoomFlipView : FlipView, ISemanticZoomInformation
    {
        /// <summary>
        /// A value that indicates whether the implementing view is the active view.
        /// </summary>
        private bool isActiveView;

        /// <summary>
        /// A value that indicates whether the implementing view is the semantically more complete zoomed-in view.
        /// </summary>
        private bool isZoomedInView;

        /// <summary>
        /// The SemanticZoom owner that hosts the implementing view.
        /// </summary>
        private SemanticZoom semanticZoomOwner;

        /// <summary>
        /// Gets or sets a value indicating whether the implementing view is the  active view.
        /// </summary>
        public bool IsActiveView
        {
            get
            {
                return this.isActiveView;
            }

            set
            {
                this.isActiveView = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the implementing view is the active view.
        /// </summary>
        public bool IsZoomedInView
        {
            get
            {
                return this.isZoomedInView;
            }

            set
            {
                this.isZoomedInView = value;
            }
        }

        /// <summary>
        /// Gets or sets the SemanticZoom owner that hosts the implementing view.
        /// </summary>
        public SemanticZoom SemanticZoomOwner
        {
            get
            {
                return this.semanticZoomOwner;
            }

            set
            {
                this.semanticZoomOwner = value;
            }
        }

        private bool _IsNavigating = false;

        public bool IsNavigating
        {
            get { return _IsNavigating; }
            set { _IsNavigating = value; }
        }

        /// <summary>
        /// Changes related aspects of presentation (such as scrolling UI or state) when the overall view for a SemanticZoom changes.
        /// </summary>
        public void CompleteViewChange()
        {
        }

        /// <summary>
        /// Completes item-wise operations related to a view change when the implementing view is the source view and the new view is a potentially different implementing view.
        /// </summary>
        /// <param name="source">The view item as represented in the source view.</param>
        /// <param name="destination">The view item as represented in the destination view.</param>
        public void CompleteViewChangeFrom(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
        }

        /// <summary>
        /// Completes item-wise operations related to a view change when the implementing view is the destination view and the source view is a potentially different implementing view.
        /// </summary>
        /// <param name="source">The view item as represented in the source view.</param>
        /// <param name="destination">The view item as represented in the destination view.</param>
        public void CompleteViewChangeTo(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
        }

        /// <summary>
        /// Initializes the changes to related aspects of presentation (such as scrolling UI or state) when the overall view for a SemanticZoom is about to change.
        /// </summary>
        public void InitializeViewChange()
        {
        }

        /// <summary>
        /// Forces content in the view to scroll until the item specified by SemanticZoomLocation is visible. Also focuses that item if found.
        /// </summary>
        /// <param name="item">The item in the view to scroll to.</param>
        public void MakeVisible(SemanticZoomLocation item)
        {
            if (null != item.Item && !IsNavigating)
                this.SelectedItem = item.Item;
        }

        /// <summary>
        /// Initializes item-wise operations related to a view change when the implementing view is the source view and the pending destination view is a potentially different implementing view.
        /// </summary>
        /// <param name="source">The view item as represented in the source view.</param>
        /// <param name="destination">The view item as represented in the destination view.</param>
        public void StartViewChangeFrom(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
        }

        /// <summary>
        /// Initializes item-wise operations related to a view change when the source view is a different view and the pending destination view is the implementing view.
        /// </summary>
        /// <param name="source">The view item as represented in the source view.</param>
        /// <param name="destination">The view item as represented in the destination view.</param>
        public void StartViewChangeTo(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
            destination.Item = source.Item;
        }
    }
}
