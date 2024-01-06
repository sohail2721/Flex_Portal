using MudBlazor;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
namespace Bismillah.Client
{
    public class MudItems
    {
        public static ISnackbar Snackbar { get; set; }

        public static void SnackBar(string message, string position, Severity severity)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = position;
            Snackbar.Add(message, severity);
        }
    }
}

