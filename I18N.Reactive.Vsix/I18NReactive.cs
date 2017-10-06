// Copyright Weingartner Maschinenbau GmbH
// MIT Licenced http://opensource.org/licenses/MIT

using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;

namespace I18N.Reactive.Vsix
{
    [ComVisible(true)]
    [Guid(GuidList.GuidI18NReactivetring)]
    [ProvideObject(typeof(I18NReactive))]
    [CodeGeneratorRegistration(typeof(I18NReactive), "I18N.Reactive", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(I18NReactive), "I18N.Reactive", "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}", GeneratesDesignTimeSource = true)]
    public class I18NReactive : IVsSingleFileGenerator, IObjectWithSite
    {
        private object _Site;

        #region IVsSingleFileGenerator

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".Designer.cs";
            return VSConstants.S_OK;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            if (bstrInputFileContents == null)
                throw new ArgumentException("bstrInputFileContents");

            var content = new I18NResX(wszDefaultNamespace, wszInputFilePath).TransformText();

            var bytes = Encoding.UTF8.GetBytes(content);

            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);
            pcbOutput = (uint)bytes.Length;

            return VSConstants.S_OK;
        }

        #endregion IVsSingleFileGenerator

        #region IObjectWithSite

        public void GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (_Site == null)
                Marshal.ThrowExceptionForHR(VSConstants.E_NOINTERFACE);

            // Query for the interface using the site object initially passed to the generator
            var punk = Marshal.GetIUnknownForObject(_Site);
            var hr = Marshal.QueryInterface(punk, ref riid, out ppvSite);
            Marshal.Release(punk);
            ErrorHandler.ThrowOnFailure(hr);
        }

        public void SetSite(object pUnkSite)
        {
            // Save away the site object for later use
            _Site = pUnkSite;

            // These are initialized on demand via our private CodeProvider and SiteServiceProvider properties
        }

        #endregion IObjectWithSite
    }
}
