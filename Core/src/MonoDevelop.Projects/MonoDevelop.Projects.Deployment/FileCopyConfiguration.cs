//
// FileCopyConfiguration.cs
//
// Author:
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (C) 2006 Michael Hutchinson
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using MonoDevelop.Core;
using MonoDevelop.Projects.Serialization;

namespace MonoDevelop.Projects.Deployment
{
	
	[DataItem (FallbackType=typeof(UnknownFileCopierConfiguration))]
	public class FileCopyConfiguration
	{
		FileCopyHandler handler;
		
		[ItemProperty ("Handler")]
		string handlerId;
		
		internal void Bind (FileCopyHandler handler)
		{
			this.handler = handler;
			this.handlerId = handler.Id;
		}
		
		public FileCopyHandler Handler {
			get {
				if (handler == null) {
					if (handlerId == null)
						throw new InvalidOperationException ("FileCopyConfiguration not bound to a handler");
					handler = Services.DeployService.GetFileCopyHandler (handlerId);
					if (handler == null)
						throw new InvalidOperationException ("FileCopyConfiguration not found: " + handlerId);
				}
				return handler;
			}
		}
		
		public void CopyFiles (IProgressMonitor monitor, IFileReplacePolicy replacePolicy, DeployFileCollection files)
		{
			Handler.CopyFiles (monitor, replacePolicy, this, files);
		}
		
		public virtual void CopyFrom (FileCopyConfiguration other)
		{
			handler = other.handler;
			handlerId = other.handlerId;
		}
		
		public FileCopyConfiguration Clone ()
		{
			FileCopyConfiguration c = Handler.CreateConfiguration ();
			c.CopyFrom (this);
			return c;
		}
	}
}
