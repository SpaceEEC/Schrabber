using Schrabber.Models;
using Schrabber.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for ProgressWindow.xaml
	/// </summary>
	public partial class ProgressWindow : Window, INotifyPropertyChanged
	{
		public readonly static DependencyProperty JobsDependencyProperty = DependencyProperty.Register(
			nameof(Jobs),
			typeof(ObservableCollection<Job>),
			typeof(ProgressWindow),
			new PropertyMetadata(null)
		);

		public ObservableCollection<Job> Jobs
		{
			get => (ObservableCollection<Job>)this.GetValue(JobsDependencyProperty);
			set => this.SetValue(JobsDependencyProperty, value);
		}

		public IEnumerable<Job> StartedJobs => this.Jobs?.Where(job => job.Started && !job.Finished) ?? Enumerable.Empty<Job>();
		public IEnumerable<Job> WaitingJobs => this.Jobs?.Where(job => !job.Started && !job.Finished) ?? Enumerable.Empty<Job>();
		public IEnumerable<Job> FinishedJobs => this.Jobs?.Where(job => job.Started && job.Finished) ?? Enumerable.Empty<Job>();

#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

		public Double Progress { get; private set; } = 0D;

		private readonly String _folderPath;
		private readonly Splitter _splitter;

		public ProgressWindow(String path, IEnumerable<Media> medias)
		{
			this.InitializeComponent();

			this.Jobs = new ObservableCollection<Job>();

			this.Jobs.CollectionChanged += this.Jobs_CollectionChanged;

			foreach (Job job in Splitter.GetJobs(medias))
				this.Jobs.Add(job);

			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Jobs)));

			this._folderPath = path;
			this._splitter = new Splitter(path, this.Jobs);
			this._splitter.Start();
		}

		private void Jobs_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (Job job in e.NewItems)
					job.PropertyChanged += this.Job_PropertyChanged;

			if (e.OldItems != null)
				foreach (Job job in e.OldItems)
					job.PropertyChanged -= this.Job_PropertyChanged;
		}

		private void Job_PropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			this.Dispatcher.Invoke(() =>
			{
				if (e.PropertyName == nameof(Job.Finished) && ((Job)sender).Finished)
				{
					this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.FinishedJobs)));
					this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.StartedJobs)));
				}
				else if (e.PropertyName == nameof(Job.Started) && ((Job)sender).Started)
				{
					this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.StartedJobs)));
					this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WaitingJobs)));
				}
				else if (e.PropertyName == nameof(Job.Progress))
				{

					Double newProgress = this.Jobs.Sum(job => job.Progress) / this.Jobs.Count;

					if (this.Progress != newProgress)
					{
						this.Progress = newProgress;

						this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Progress)));
					}
				}
			});
		}

		private void OpenDestinationButton_Click(Object sender, RoutedEventArgs e)
			=> Process.Start(this._folderPath);
	}
}
