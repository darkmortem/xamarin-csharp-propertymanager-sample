﻿/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */

using System;
using System.Drawing;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Platform.iOS.Platform;
using XamarinNativePropertyManager.Models;
using XamarinNativePropertyManager.ViewModels;
using UIKit;
using XamarinNativePropertyManager.iOS.Views.Cells;

namespace XamarinNativePropertyManager.iOS.Views.Tabs
{
	public class ConversationsTabViewSource : MvxTableViewSource
	{
		private readonly MvxIosMajorVersionChecker _iosVersion6Checker = new MvxIosMajorVersionChecker(6);

		public GroupViewModel ViewModel { get; }

		public ConversationsTabViewSource(UITableView tableView, GroupViewModel viewModel, NSBundle bundle = null)
			: base(tableView)
		{
			ViewModel = viewModel;

			// Register nibs.
			tableView.RegisterNibForCellReuse(UINib.FromName(ConversationsTableLeftViewCell.Key, bundle ?? NSBundle.MainBundle),
											  ConversationsTableLeftViewCell.Key);
			tableView.RegisterNibForCellReuse(UINib.FromName(ConversationsTableRightViewCell.Key, bundle ?? NSBundle.MainBundle),
											  ConversationsTableRightViewCell.Key);

		}

		protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
		{
		    var conversationModel = item as ConversationModel;
		    var key = conversationModel != null && conversationModel.IsOwnedByUser 
			                                     ? ConversationsTableRightViewCell.Key 
			                                     : ConversationsTableLeftViewCell.Key;
			return _iosVersion6Checker.IsVersionOrHigher 
                ? tableView.DequeueReusableCell(key, indexPath) 
                : tableView.DequeueReusableCell(key);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			// Get the conversation.
			var conversation = ViewModel.Conversations[indexPath.Row];

			// Get the cell and set values.
			var cell = tableView.DequeueReusableCell(ConversationsTableLeftViewCell.Key) as ConversationsTableLeftViewCell;
		    if (cell == null)
		    {
                return 0;
            }
		    cell.SetValues(conversation.Preview, conversation.UniqueSenders[0]);

		    // Update the constraints.
		    cell.SetNeedsUpdateConstraints();
		    cell.UpdateConstraintsIfNeeded();

		    cell.Bounds = new RectangleF(0, 0, (float)TableView.Bounds.Width, (float)TableView.Bounds.Height);

		    // Update the layout.
		    cell.SetNeedsLayout();
		    cell.LayoutIfNeeded();
		    cell.LayoutSubviews();

		    // Get the height.
		    var height = cell.ContentView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize).Height;
		    return height + 1;
		}
	}
}

