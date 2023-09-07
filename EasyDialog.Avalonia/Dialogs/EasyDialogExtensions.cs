﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.Layout;
using DialogHostAvalonia;
using EasyDialog.Avalonia.Loading;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDialog.Avalonia.Dialogs;

public static class EasyDialogExtensions
{
#if DEBUG
    public static readonly DialogService DialogService = new DialogService();

#else
    internal static readonly DialogService DialogService = new DialogService();
#endif
    /// <summary>
    /// AddDialogManager
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public static IServiceCollection AddEasyDialog(this IServiceCollection service)
    {
        service.AddSingleton<DialogService>(s => { return DialogService; });
        return service;
    }

    public static UserControl? UseEasyDialog(this UserControl view, string identifier = null)
    {

        var content = view.Content;
        view.Content = null;
        view.Content = InjectContent(content, identifier);
        return view;
    }

    public static Window UseEasyDialog(this Window window, string identifier = null)
    {
        var content = window.Content;
        window.Content = null;
        window.Content = InjectContent(content, identifier);
        return window;
    }

    static Control? InjectContent(object? content, string identifier = null)
    {

        var id = identifier ?? DialogConsts.MainViewDefaultIdentifier;
        
        var loadingContainer = new EasyDialogLoadingContainer()
        {
            ZIndex = 100,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch,
            Content = new DialogHost()
            {
                Identifier = id,
                Content = content
            }
        };

        DialogService.OnDialogShowLoadingHandler += (identifier, options, isLoading) =>
        {
            if (identifier == id)
            {
                options?.Invoke(loadingContainer);
                loadingContainer.IsLoading = isLoading;
            }
        };

        return loadingContainer;
    }
}