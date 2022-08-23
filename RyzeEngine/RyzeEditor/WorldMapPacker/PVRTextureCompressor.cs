using System.Diagnostics;

namespace RyzeEditor.Packer
{
    public class PVRTextureCompressor : ITextureCompressor
    {
        public void Compress(string file, int mipmaps)
        {
            var fileFormat = "PVRTC1_4";

            var args = $"-m {mipmaps} -f {fileFormat} -i {file} -square -pot";

            var processInfo = new ProcessStartInfo("PVRTexTool.exe", args)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var process = Process.Start(processInfo);

            process.WaitForExit(120 * 1000);
        }
    }
}
