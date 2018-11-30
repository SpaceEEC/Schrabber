using Schrabber.Models;
using System;

namespace Schrabber.Workers
{
	public class Job : ViewModelBase, IProgress<Double>
	{
		public Job(Object target)
		{
			this._target = target;
		}

		private Object _target = default;
		public Object Target
		{
			get => this._target;
			set => this.SetProperty(ref this._target, value);
		}

		private Double _progress = 0D;
		public Double Progress
		{
			get => this._progress;
			set => this.SetProperty(ref this._progress, value);
		}

		private String _caption = Properties.Resources.Job_Waiting;
		public String Caption
		{
			get => this._caption;
			set => this.SetProperty(ref this._caption, value);
		}

		private Boolean _started = false;
		public Boolean Started
		{
			get => this._started;
			set => this.SetProperty(ref this._started, value);
		}

		private Boolean _finished = false;
		public Boolean Finished
		{
			get => this._finished;
			set
			{
				if (!this.Started) this.Started = true;
				this.Caption = Properties.Resources.Job_Done;
				this.Progress = 1D;
				this.SetProperty(ref this._finished, value);
			}
		}

		public void Report(Double value) => this.Progress = value;
	}
}
