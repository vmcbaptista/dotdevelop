//
// Copyright (c) Microsoft Corp. (https://www.microsoft.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Ide.Composition;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.Ide.Text
{
	class TextViewDisplayBinding : IViewDisplayBinding
	{
		public string Name => GettextCatalog.GetString ("Source Code Editor");

		public bool CanUseAsDefault => true;

		public bool CanHandle (FilePath fileName, string mimeType, Project ownerProject)
		{
			if (fileName == null || !(IsSupportedFileExtension (fileName) || IsSupportedAndroidFileName (fileName, ownerProject))) {
				return false;
			}

			if (fileName != null)
				return DesktopService.GetFileIsText (fileName, mimeType);

			if (!string.IsNullOrEmpty (mimeType))
				return DesktopService.GetMimeTypeIsText (mimeType);

			return false;
		}

		static HashSet<string> supportedFileExtensions = new HashSet<string> (StringComparer.OrdinalIgnoreCase) {
			".cs",
			".html",
			".cshtml",
			".css",
			".json",
			".js",
			".ts"
		};

		bool IsSupportedFileExtension (FilePath fileName)
		{
			return supportedFileExtensions.Contains (fileName.Extension);
		}

		bool IsSupportedAndroidFileName (FilePath fileName, Project ownerProject)
		{
			// We only care about .xml and .axml files that are marked as AndroidResource
			if (!(fileName.HasExtension (".xml") || fileName.HasExtension (".axml")))
				return false;

			const string AndroidResourceBuildAction = "AndroidResource";
			var buildAction = ownerProject.GetProjectFile (fileName)?.BuildAction;
			return string.Equals (buildAction, AndroidResourceBuildAction, System.StringComparison.Ordinal);
		}

		public ViewContent CreateContent (FilePath fileName, string mimeType, Project ownerProject)
		{
			var imports = CompositionManager.GetExportedValue<TextViewImports> ();
			var viewContent = new TextViewContent (imports, fileName, mimeType, ownerProject);
			return viewContent;
		}
	}
}