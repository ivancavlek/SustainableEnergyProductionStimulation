//using System.Linq;
//using System.Reflection;

//namespace Acme.Seps.Bootstrap
//{
//    internal abstract class AbstractionsWithImplementation
//    {
//        protected readonly char[] _onDot;
//        protected readonly string _projectName;
//        protected readonly string _executingProjectName;

//        internal AbstractionsWithImplementation()
//        {
//            _onDot = ".".ToCharArray();
// big problem of finding the calling assembly, practically impossible, only sending per ctor
//            var currentAssemblyPartedFullName = Assembly.GetExecutingAssembly().GetName().Name.Split(_onDot);
//            _projectName = currentAssemblyPartedFullName[0];
//            _executingProjectName = currentAssemblyPartedFullName.Last();
//        }
//    }
//}