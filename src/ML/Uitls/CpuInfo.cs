using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ML.Utils
{
    /// <summary>
    /// Retrieves information about the processor.
    /// </summary>
    public class Processor
    {
        static Processor()
        {
            //Console.WriteLine("static Processor()");
            string dllDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { Path.DirectorySeparatorChar });
            string win32CpuInfoFilePath = Path.Combine(dllDir, "cpuinfo.dll");
            ReleaseFile(win32CpuInfoFilePath, LibResource.cpuinfo);
        }
        static void ReleaseFile(string filePath, byte[] content)
        {
            var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fs.Write(content, 0, content.Length);
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// Retrieves a 12-byte string that contains the Vendor ID (ie. 'GenuineIntel', 'AuthenticAMD', ...)
        /// </summary>
        /// <param name="buffer">The string buffer that will receive the vendor ID.</param>
        /// <remarks>The buffer must be at least 13 characters long [12 + null char].</remarks>
        [DllImport(@"cpuinfo.dll")]
        private static extern void GetVendorID(IntPtr buffer);
        /// <summary>
        /// Returns an integer that contains the processor features.
        /// </summary>
        /// <remarks>
        /// The following list is a description of the bits and their meaning:
        ///		bit 31 reserved 
        ///		bit 30 (IA-64) 
        ///		bit 29 (TM) Thermal monitor supported
        ///		bit 28 (HTT) Hyper-Threading Technology 
        ///		bit 27 (SS) selfsnoop 
        ///		bit 26 (SSE2) Streaming SIMD extensions 2 supported
        ///		bit 25 (SSE) Streaming SIMD extensions supported
        ///		bit 24 (FXSR) Fast floating point save and restore
        ///		bit 23 (MMX) MMX supported
        ///		bit 22 (ACPI) 
        ///		bit 21 (DTES) Debug Trace and EMON Store MSRs 
        ///		bit 20 reserved 
        ///		bit 19 (CLFL) CLFLUSH instruction supported
        ///		bit 18 (PSN) Processor serial number present and enabled
        ///		bit 17 (PSE36) 36-bit page size extension
        ///		bit 16 (PAT) Page attribute table
        ///		bit 15 (CMOV) Conditional move instruction supported
        ///		bit 14 (MCA) Machine check architecture
        ///		bit 13 (PGE) Page global enabled
        ///		bit 12 (MTRR) Memory type range registers
        ///		bit 11 (SEP) 
        ///		bit 10 reserved 
        ///		bit 9 (APIC) On-chip APIC hardware supported
        ///		bit 8 (CX8) SMPXCHG instruction supported
        ///		bit 7 (MCE) Machine check extension
        ///		bit 6 (PAE) Physical address extension
        ///		bit 5 (MSR) Model specific registers
        ///		bit 4 (TSC) Time stamp counter
        ///		bit 3 (PSE) Page size extension
        ///		bit 2 (DE) Debugging extension
        ///		bit 1 (VME) Virtual Mode extension
        ///		bit 0 (FPU) Floating point unit on-chip
        /// </remarks>
        /// <returns>An integer value.</returns>
        [DllImport(@"cpuinfo.dll")]
        private static extern int GetProcessorFeatures();
        /// <summary>
        /// Checks for 3DNow! support.
        /// </summary>
        /// <returns>A non-zero value if the processor supports 3DNow, zero otherwise.</returns>
        [DllImport(@"cpuinfo.dll", EntryPoint = "Has3DNow")]
        private static extern int Has3DNowAsm();
        /// <summary>
        /// Checks for 3DNow! Extensions support.
        /// </summary>
        /// <returns>A non-zero value if the processor supports 3DNow Extensions, zero otherwise.</returns>
        [DllImport(@"cpuinfo.dll", EntryPoint = "Has3DNowExt")]
        private static extern int Has3DNowExtAsm();
        /// <summary>
        /// Checks for MMX support.
        /// </summary>
        /// <returns>A non-zero value if the processor supports MMX, zero otherwise.</returns>
        [DllImport(@"cpuinfo.dll")]
        private static extern int HasMmx();
        /// <summary>
        /// Checks for the presence of a processor serial number.
        /// </summary>
        /// <returns>A non-zero value if the processor has a serial number, zero otherwise.</returns>
        [DllImport(@"cpuinfo.dll")]
        private static extern int HasSerialNumber();
        /// <summary>
        /// Returns the processor family number.
        /// </summary>
        /// <returns>An integer value.</returns>
        [DllImport(@"cpuinfo.dll")]
        private static extern int GetProcessorFamily();
        /// <summary>
        /// Returns the processor model number.
        /// </summary>
        /// <returns>An integer value.</returns>
        [DllImport(@"cpuinfo.dll")]
        private static extern int GetProcessorModel();
        /// <summary>
        /// Returns the processor stepping number.
        /// </summary>
        /// <returns>An integer value.</returns>
        [DllImport(@"cpuinfo.dll")]
        private static extern int GetProcessorStepping();
        /// <summary>
        /// Retrieves the processor serial number (if it has one).
        /// </summary>
        /// <param name="serial">The variable that receives the serial number.</param>
        /// <returns>Returns zero if an error occurred, a non-zero value otherwise.</returns>
        /// <remarks>Processor manufacturers cannot guarantee that a processor serial number is unique.</remarks>
        [DllImport(@"cpuinfo.dll")]
        private static extern int GetProcessorSerial(out long serial);
        /// <summary>
        /// Retrieves the name of the processor.
        /// </summary>
        /// <param name="buffer">The string buffer that receives the processor name.</param>
        /// <remarks>
        /// The size of the buffer must be at least 49 characters (48 + NULL char).
        /// The returned buffer can be empty on processors that do not support the processor name function (Intel Pentium III and older, for instance).
        /// </remarks>
        [DllImport(@"cpuinfo.dll")]
        private static extern void GetProcessorName(IntPtr buffer);
        /// <summary>
        /// Initializes a new Processor instance.
        /// </summary>
        public Processor()
        {
            //Console.WriteLine("public Processor()");
        }
        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        /// <value>A string that contains the name of the processor.</value>
        public string Name
        {
            get
            {
                if (m_Name == null)
                {
                    IntPtr name = Marshal.AllocHGlobal(49);
                    GetProcessorName(name);
                    m_Name = Marshal.PtrToStringAnsi(name);
                    Marshal.FreeHGlobal(name);
                }
                return m_Name;
            }
        }
        /// <summary>
        /// Gets the name of the vendor of the processor.
        /// </summary>
        /// <value>A string that contains the name of the vendor.</value>
        public string Vendor
        {
            get
            {
                if (m_Vendor == null)
                {
                    IntPtr vendor = Marshal.AllocHGlobal(13);
                    GetVendorID(vendor);
                    m_Vendor = Marshal.PtrToStringAnsi(vendor);
                    Marshal.FreeHGlobal(vendor);
                }
                return m_Vendor;
            }
        }
        /// <summary>
        /// Gets a <see cref="Version"/> instance that contains the processor build number.
        /// </summary>
        /// <value>An instance of the Version class.</value>
        /// <remarks>Major corresponds to Family, Minor corresponds to Model and Build corresponds to Stepping.</remarks>
        public Version Version
        {
            get
            {
                if (m_Version == null)
                {
                    m_Version = new Version(Family, Model, Stepping);
                }
                return m_Version;
            }
        }
        /// <summary>
        /// Gets the family of the processor.
        /// </summary>
        /// <value>An integer that contains the family number of the processor.</value>
        public int Family
        {
            get
            {
                return GetProcessorFamily();
            }
        }
        /// <summary>
        /// Gets the model of the processor.
        /// </summary>
        /// <value>An integer that contains the model number of the processor.</value>
        public int Model
        {
            get
            {
                return GetProcessorModel();
            }
        }
        /// <summary>
        /// Gets the stepping of the processor.
        /// </summary>
        /// <value>An integer that contains the stepping number of the processor.</value>
        public int Stepping
        {
            get
            {
                return GetProcessorStepping();
            }
        }
        /// <summary>
        /// Gets the processor features.
        /// </summary>
        /// <value>A combination of the ProcessorFeatures fields.</value>
        public ProcessorFeatures Features
        {
            get
            {
                return (ProcessorFeatures)GetProcessorFeatures();
            }
        }
        /// <summary>
        /// Gets the serial number of the processor.
        /// </summary>
        /// <value>A long that contains the serial number of the processor.</value>
        /// <exception cref="NotSupportedException">The processor does not have a serial number.</exception>
        public long Serial
        {
            get
            {
                long ret;
                if (HasSerialNumber() == 0 || GetProcessorSerial(out ret) == 0)
                    throw new NotSupportedException("This processor does not have a serial number, or it is not enabled.");
                return ret;
            }
        }
        /// <summary>
        /// Checks whether the processor supports <i>3D Now!</i>.
        /// </summary>
        /// <value><b>true</b> if the processor supports <i>3D Now!</i>, false otherwise.</value>
        public bool Has3DNow()
        {
            return Has3DNowAsm() != 0;
        }
        /// <summary>
        /// Checks whether the processor supports <i>3D Now!</i> extensions.
        /// </summary>
        /// <value><b>true</b> if the processor supports <i>3D Now!</i> extensions, false otherwise.</value>
        public bool Has3DNowExt()
        {
            return Has3DNowExtAsm() != 0;
        }
        private string m_Name;
        private string m_Vendor;
        private Version m_Version;
    }
    /// <summary>
    /// Enumerates the possible processor features
    /// </summary>
    public enum ProcessorFeatures : int
    {
        /// <summary>Floating point unit on-chip</summary>
        FPU = 0x1,
        /// <summary>Virtual Mode extension</summary>
        VME = 0x2,
        /// <summary>Debugging extension</summary>
        DE = 0x4,
        /// <summary>Page size extension</summary>
        PSE = 0x8,
        /// <summary>Time stamp counter</summary>
        TSC = 0x10,
        /// <summary>Model specific registers</summary>
        MSR = 0x20,
        /// <summary>Physical address extension</summary>
        PAE = 0x40,
        /// <summary>Machine check extension</summary>
        MCE = 0x80,
        /// <summary>SMPXCHG instruction supported</summary>
        CX8 = 0x100,
        /// <summary>On-chip APIC hardware supported</summary>
        APIC = 0x200,
        // bit 10 == reserved
        /// <summary>SEP supported</summary>
        SEP = 0x800,
        /// <summary>Memory type range registers</summary>
        MTRR = 0x1000,
        /// <summary>Page global enabled</summary>
        PGE = 0x2000,
        /// <summary>Machine check architecture</summary>
        MCA = 0x4000,
        /// <summary>Conditional move instruction supported</summary>
        CMOV = 0x8000,
        /// <summary>Page attribute table</summary>
        PAT = 0x10000,
        /// <summary>36-bit page size extension</summary>
        PSE36 = 0x20000,
        /// <summary>Processor serial number present and enabled</summary>
        PSN = 0x40000,
        /// <summary>CLFLUSH instruction supported</summary>
        CLFL = 0x80000,
        // bit 20 == reserved
        /// <summary>Debug Trace and EMON Store MSRs</summary>
        DTES = 0x200000,
        /// <summary>ACPI supported</summary>
        ACPI = 0x400000,
        /// <summary>MMX supported</summary>
        MMX = 0x800000,
        /// <summary>Fast floating point save and restore</summary>
        FXSR = 0x1000000,
        /// <summary>Streaming SIMD extensions supported</summary>
        SSE = 0x2000000,
        /// <summary>Streaming SIMD extensions 2 supported</summary>
        SSE2 = 0x4000000,
        /// <summary>selfsnoop</summary>
        SS = 0x8000000,
        /// <summary>Hyper-Threading Technology</summary>
        HTT = 0x10000000,
        /// <summary>Thermal monitor supported</summary>
        TM = 0x20000000,
        /// <summary>IA-64</summary>
        IA64 = 0x40000000
        // bit 31 == reserved
    }
}
