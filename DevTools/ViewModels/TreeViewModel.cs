using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DevTools.Contracts.ViewModels;
using DevTools.Core.Contracts.Services;
using DevTools.Core.Models;

namespace DevTools.ViewModels;

public class TreeViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;
    private SampleOrder? _selected;
    private MessageInfo? _messageInfo;

    public SampleOrder? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new ObservableCollection<SampleOrder>();
    public MessageInfo MessageInfos {
        get => _messageInfo;
        set =>SetProperty(ref _messageInfo, value);
    } 


    public TreeViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        SampleItems.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetListDetailsDataAsync();

        foreach (var item in data)
        {
            SampleItems.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        if (Selected == null)
        {
            Selected = SampleItems.First();
        }
    }
}
