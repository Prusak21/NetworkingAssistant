using AsystentSieciowca.Modules;

namespace AsystentSieciowca
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var calc = new SubnetCalculatorModule();
            var conf = new ConfigGeneratorModule();
            var edu = new EducationModule();

            var app = new ApplicationFacade(calc, conf, edu);

            app.Start();
        }
    }
}
