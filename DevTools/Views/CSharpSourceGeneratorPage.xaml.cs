
using System.Text.Json.Nodes;
using Windows.ApplicationModel.DataTransfer;
using DevTools.Core.Models;
using DevTools.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace DevTools.Views;
public sealed partial class CSharpSourceGeneratorPage:Page
{
    public CSharpSourceGeneratorPage()
    {
        this.InitializeComponent();
    }
}