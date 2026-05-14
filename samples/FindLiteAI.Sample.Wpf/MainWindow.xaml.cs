using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Enums;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FindLiteAI.Sample.Wpf;

/// <summary>
/// Demonstrates FindLiteAI usage inside a WPF desktop application.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Stores the search results displayed in the results grid.
    /// </summary>
    private readonly ObservableCollection<SearchResultViewModel> _results = [];

    /// <summary>
    /// The semantic search engine instance used by the sample.
    /// </summary>
    private ISemanticSearchEngine? _searchEngine;

    /// <summary>
    /// The current service provider instance.
    /// </summary>
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        ResultsGrid.ItemsSource = _results;

        ModelComboBox.ItemsSource =
            FindLiteAIModels.GetAll();

        ModelComboBox.SelectedItem =
            FindLiteAIModels.MiniLm;
    }

    /// <summary>
    /// Loads the selected model package and initializes the search engine.
    /// </summary>
    private async void LoadModelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (ModelComboBox.SelectedItem is not FindLiteAIModelDefinition model)
        {
            StatusTextBlock.Text = "Select a model first.";
            return;
        }

        try
        {
            SetBusy($"Installing model package: {model.DisplayName}...");

            string cacheDirectory =
                Path.Combine(
                    Path.GetTempPath(),
                    "FindLiteAI",
                    "Models");

            await ModelInstallService.InstallAsync(
                model,
                cacheDirectory,
                overwrite: false);

            string modelDirectory =
                Path.Combine(
                    cacheDirectory,
                    model.Id);

            ServiceCollection services = new();

            services.AddFindLiteAI(options =>
            {
                options.DatabasePath = $"findliteai-wpf-sample-{model.Id}.db";
                options.ModelCacheDirectory = modelDirectory;
            });

            if (_serviceProvider is not null)
            {
                await _serviceProvider.DisposeAsync();
            }

            _serviceProvider =
                services.BuildServiceProvider();

            _searchEngine =
                _serviceProvider.GetRequiredService<ISemanticSearchEngine>();

            SetBusy("Indexing sample documents...");

            await _searchEngine.AddRangeAsync(
                "wpf_logs",
                [
                    new SemanticDocument
                    {
                        Id = "wpf-log-1",
                        Text = "SFTP authentication failed for remote user."
                    },
                    new SemanticDocument
                    {
                        Id = "wpf-log-2",
                        Text = "SQL database timeout occurred while executing query."
                    },
                    new SemanticDocument
                    {
                        Id = "wpf-log-3",
                        Text = "SMTP email relay accepted outgoing message."
                    },
                    new SemanticDocument
                    {
                        Id = "wpf-log-4",
                        Text = "All system issues include login failures, database timeouts, and SMTP email errors."
                    }
                ]);

            StatusTextBlock.Text =
                $"Loaded {model.DisplayName}. Enter a query and click Search.";

            await SearchAsync();
        }
        catch (Exception exception)
        {
            StatusTextBlock.Text = exception.Message;

            MessageBox.Show(
                exception.ToString(),
                "FindLiteAI WPF Sample Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            SetIdle();
        }
    }

    /// <summary>
    /// Executes a search when the Search button is clicked.
    /// </summary>
    private async void SearchButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await SearchAsync();
    }

    /// <summary>
    /// Executes a search when the Enter key is pressed.
    /// </summary>
    private async void SearchTextBox_KeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await SearchAsync();
        }
    }

    /// <summary>
    /// Executes a hybrid search using the current query text and updates the results grid.
    /// </summary>
    private async Task SearchAsync()
    {
        if (_searchEngine is null)
        {
            StatusTextBlock.Text = "Load a model before searching.";
            return;
        }

        string query =
            SearchTextBox.Text;

        if (string.IsNullOrWhiteSpace(query))
        {
            StatusTextBlock.Text = "Enter a search query.";
            return;
        }

        try
        {
            SetBusy("Searching...");

            IReadOnlyList<SearchResult> results =
                await _searchEngine.SearchAsync(
                    "wpf_logs",
                    query,
                    new SearchOptions
                    {
                        SearchMode = SearchMode.Hybrid,
                        MaxResults = 5,
                        MinimumScore = 0.50
                    });

            _results.Clear();

            foreach (SearchResult result in results)
            {
                _results.Add(
                    new SearchResultViewModel
                    {
                        Rank = result.Rank,
                        Score = result.Score.ToString("0.0000"),
                        Id = result.Document.Id,
                        Text = result.Document.Text
                    });
            }

            StatusTextBlock.Text =
                $"Found {results.Count} result(s).";
        }
        catch (Exception exception)
        {
            StatusTextBlock.Text = exception.Message;

            MessageBox.Show(
                exception.ToString(),
                "Search Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            SetIdle();
        }
    }

    /// <summary>
    /// Updates the UI to indicate a busy operation.
    /// </summary>
    private void SetBusy(
        string message)
    {
        StatusTextBlock.Text = message;
        SearchButton.IsEnabled = false;
        LoadModelButton.IsEnabled = false;
        ModelComboBox.IsEnabled = false;
    }

    /// <summary>
    /// Restores the UI controls to the idle state after an operation completes.
    /// </summary>
    private void SetIdle()
    {
        SearchButton.IsEnabled = true;
        LoadModelButton.IsEnabled = true;
        ModelComboBox.IsEnabled = true;
    }

    /// <summary>
    /// Represents a search result displayed in the grid.
    /// </summary>
    private sealed class SearchResultViewModel
    {
        public int Rank { get; set; }

        public string Score { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
