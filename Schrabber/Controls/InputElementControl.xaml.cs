using Schrabber.Interfaces;
using Schrabber.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Schrabber.Controls
{
	/// <summary>
	/// Interaction logic for InputElementControl.xaml
	/// </summary>
	public partial class InputElementControl : UserControl
	{
		/// <summary>
		/// The IInputMedia this InputElementControl was instantiated with.
		/// </summary>
		public IInputMedia Media { get; }

		private IRemoveChildElement _parent;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent">Parent this InputElementControl will be added to.</param>
		/// <param name="media">The IInputMedia to associate this InputElementControl with.</param>
		public InputElementControl(IInputMedia media, IRemoveChildElement parent)
		{
			InitializeComponent();
			_parent = parent;
			Media = media;

			if (media.CoverImage != null) ThumbnailImage.Source = media.CoverImage;
			InformationTextBlock.Text = String.IsNullOrWhiteSpace(media.Author) ? $"{media.Author} - " : "";
			InformationTextBlock.Text += $"{media.Title}\n\n{media.Description}";
			_updateButton();
		}

		private void _updateButton() => SplitButton.Content = $"Split\n\nParts: {Media.Parts.Length}";

		private void SplitButton_Click(object sender, RoutedEventArgs e)
		{
			SplitWindow window = new SplitWindow(Media);
			if (window.ShowDialog() != true) return;
			IPart[] parts = window.Parts.ToArray();
			for (Int32 i = 0; i > parts.Length; ++i)
			{
				if (i + 1 == parts.Length)
				{
					parts[i].Stop = null;
					break;
				}

				parts[i].Stop = parts[i + 1].Start;
			}

			Media.Parts = parts;

			_updateButton();
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			_parent.RemoveChildElement(this);
			Media?.Dispose();
		}
	}
}
