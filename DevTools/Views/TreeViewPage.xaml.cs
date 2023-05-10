// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Text.Json.Nodes;
using Windows.ApplicationModel.DataTransfer;
using DevTools.Core.Models;
using DevTools.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonException = System.Text.Json.JsonException;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace DevTools.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TreeViewPage : Page
{

    public TreeViewModel ViewModel
    {
        get;
    }

    public TreeViewPage()
    {
        ViewModel = App.GetService<TreeViewModel>();
        this.InitializeComponent();
    }

    private void IterateJsonArray(JsonArray jsonArray, TreeViewNode treeViewNode)
    {
        for (var index = 0; index < jsonArray.Count; index++)
        {
            TreeViewNode t1 = new TreeViewNode { Content = "item["+index+"]" };
            treeViewNode.Children.Add(t1);
            var e = jsonArray[(Index)index];
            if (e is JsonObject)
            {
                IterateJsonObject(e.AsObject(), t1);
            }
            else if (e is JsonArray)
            {
                IterateJsonArray(e.AsArray(), t1);
            }
            else
            {
                TreeViewNode t = new TreeViewNode() { Content = e.ToString() };
                treeViewNode.Children.Add(t);
            }
        }
    }

    private void IterateJsonObject(JsonObject jsonObject, TreeViewNode treeViewNode)
    {
        foreach (var e in jsonObject)
        {
            var valueObj = e.Value;
            if (valueObj is JsonArray)
            {
                TreeViewNode t = new TreeViewNode() { Content = e.Key };
                treeViewNode.Children.Add(t);
                if (valueObj.AsArray().Count == 0)
                {
                    t.Content = e.Key + ":[]";
                    continue;
                }

                IterateJsonArray(valueObj.AsArray(), t);
            }
            else if (valueObj is JsonObject)
            {
                TreeViewNode t = new TreeViewNode() { Content = e.Key };
                treeViewNode.Children.Add(t);
                IterateJsonObject(valueObj.AsObject(), t);
            }
            else
            {
                TreeViewNode t = new TreeViewNode() { Content = e.Key + ":" + valueObj };
                treeViewNode.Children.Add(t);
            }
        }
    }

    private void TabView_AddTabButtonClick(TabView sender, object args)
    {
        var newTab = new TabViewItem();
        newTab.IconSource = new SymbolIconSource() { Symbol = Symbol.Document };
        newTab.Header = "New Document";
        // The Content of a TabViewItem is often a frame which hosts a page.
        // Frame frame = new Frame();
        // newTab.Content = frame;
        // frame.Navigate(typeof(TreeViewPage));

        sender.TabItems.Add(newTab);
    }

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
    }

    private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel.MessageInfos = new MessageInfo();
        if (sender == null)
        {
            return;
        }

        treeView.RootNodes.Clear();
        string jsonText = (sender as TextBox).Text;
        Console.WriteLine($"jsonText:{jsonText}");
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            return;
        }
        JsonNode node;
        try
        {
             node = JsonNode.Parse(jsonText);
        }
        catch (JsonException exception)
        {
            ViewModel.MessageInfos = new MessageInfo()
            {
                IsOpen = true,
                Message = exception.ToString()
            };
            Console.WriteLine(exception);
            //throw;
            return;
        }
      
        TreeViewNode myTreeViewNode = new TreeViewNode() { Content = "JSON" };
        if (node is JsonArray)
        {
            JsonArray jsonObject = node.AsArray();
            IterateJsonArray(jsonObject, myTreeViewNode);
        }
        else if (node is JsonObject)
        {
            JsonObject jsonObject = node.AsObject();
            IterateJsonObject(jsonObject, myTreeViewNode);
        }

        treeView.RootNodes.Add(myTreeViewNode);
    }

    private void CopyBarButton_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine(JsonTextBox.Text);
        var package = new DataPackage();
        package.SetText(JsonTextBox.Text);
        Clipboard.SetContent(package);
    }

    private async void PastyBarButton_Click(object sender, RoutedEventArgs e)
    {
        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Text))
        {
             JsonTextBox.Text = await package.GetTextAsync();
        }
    }

    private void FormatBarButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(JsonTextBox.Text))
        {
            return;
        }
        try
        {
            JsonTextBox.Text = JObject.Parse(JsonTextBox.Text).ToString();
        }
        catch (JsonReaderException exception)
        {
            ViewModel.MessageInfos = new MessageInfo()
            {
                IsOpen = true,
                Message = exception.ToString()
            };
        }
    }

    private void CompressBarButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            JsonTextBox.Text = JObject.Parse(JsonTextBox.Text).ToString(Formatting.None);
        }
        catch (JsonReaderException exception)
        {
            ViewModel.MessageInfos = new MessageInfo()
            {
                IsOpen = true,
                Message = exception.ToString()
            };
        }
    }

    private void CSharpBarButton_Click(object sender, RoutedEventArgs e)
    {
        TabView.TabItems.Add(new TabViewItem()
        {
            IsSelected = true,
            Header = "JSON C# Class Generator"
        });
    }
}