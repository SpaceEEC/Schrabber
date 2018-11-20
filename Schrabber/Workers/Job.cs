using Schrabber.Models;
using System;

namespace Schrabber.Workers
{
	public class Job<T> : ViewModelBase, IProgress<Double>
	{
		public Job(T target)
		{
			this._target = target;
		}

		private T _target = default;
		public T Target
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

		private String _caption = String.Empty;
		public String Caption
		{
			get => this._caption;
			set => this.SetProperty(ref this._caption, value);
		}

		public void Report(Double value) => this.Progress = value;
	}
}
