using Schrabber.Models;
using System;

namespace Schrabber.Workers
{
	internal class Job<T> : ViewModelBase, IProgress<Double>
	{
		internal Job(T target)
		{
			this._target = target;
		}

		private T _target = default;
		internal T Target
		{
			get => this._target;
			set => this.SetProperty(ref this._target, value);
		}

		private Double _progress = 0D;
		internal Double Progress
		{
			get => this._progress;
			set => this.SetProperty(ref this._progress, value);
		}

		private String _caption = String.Empty;
		internal String Caption
		{
			get => this._caption;
			set => this.SetProperty(ref this._caption, value);
		}

		public void Report(Double value) => this.Progress = value;
	}
}
