using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Extensions.AI;
using OllamaSharp;
using CommunityToolkit.Mvvm;

namespace deepseek_WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string endpointUri = "http://localhost:11434/";
        private readonly string AImodel = "deepseek-r1";

        private IChatClient chatClient;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize Ollama client
            try
            {
                chatClient = new OllamaChatClient(new Uri(endpointUri), AImodel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize AI client: {ex.Message}",
                                "Initialization Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private async void AskButton(object sender, RoutedEventArgs e)
        {
            string userInput = InputTextBox.Text;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Please enter a prompt",
                                "Input Required",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, userInput)
            };

            var responseBuilder = new StringBuilder();

            try
            {
                await foreach (var responsePart in chatClient.CompleteStreamingAsync(chatHistory))
                {
                    responseBuilder.Append(responsePart.Text);
                }

                OutputTextBlock.Text = responseBuilder.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error communicating with the AI model: {ex.Message}",
                                "Communication Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
