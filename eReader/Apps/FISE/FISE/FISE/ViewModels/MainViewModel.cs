using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
        }

        private Command<string> sampleCommand;
        public Command<string> SampleCommand
        {
            get { return sampleCommand ?? (sampleCommand = new Command<string>(async arg => await ExecuteSampleCommand(arg))); }
        }

        private async Task ExecuteSampleCommand(string arg)
        {
            try
            {
                //Console.WriteLine("ViewModel Command: " + arg);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
