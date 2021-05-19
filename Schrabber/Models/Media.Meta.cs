using System;

namespace Schrabber.Models
{
	public abstract partial class Media : Base
	{
		protected TimeSpan _duration = TimeSpan.Zero;
		public TimeSpan Duration
		{
			get => this._duration;
			set => this.SetProperty(ref this._duration, value);
		}

		protected String _description = String.Empty;
		public String Description
		{
			get => this._description;
			set => this.SetProperty(ref this._description, value);
		}

		protected Part[] _parts = null;
		public Part[] Parts
		{
			get => (this._parts?.Length ?? 0) == 0
				? this._parts = new[] { new Part(this) }
				: this._parts;
			set => this.SetProperty(ref this._parts, value);
		}
	}
}
