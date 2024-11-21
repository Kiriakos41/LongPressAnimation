namespace LongPressAnimation;

public partial class MainPage : ContentPage
{
    private const double ButtonWidth = 200; // Width of the button
    private const double ProgressIncrement = 1; // Width increment value
    private const int UpdateInterval = 10; // Milliseconds interval
    private const int MaxProgress = 100; // Maximum progress value

    private bool isPressed = false;
    private double currentProgress = 0;
    private bool isUnloading = false; // Flag to track if unloading is happening

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        LongPressButton.BindingContext = this;
    }

    private async void StartProgress()
    {
        // If unloading is happening, stop it and continue from where it was left off
        if (isUnloading)
        {
            isUnloading = false; // Stop unloading
        }

        // Progress continues from the current progress if user presses again
        while (isPressed && currentProgress < MaxProgress)
        {
            currentProgress += ProgressIncrement;

            // Ensure the progress doesn't go over 100%
            if (currentProgress > MaxProgress)
            {
                currentProgress = MaxProgress;
            }

            ProgressOverlay.WidthRequest = (currentProgress / MaxProgress) * ButtonWidth;

            if (currentProgress >= MaxProgress)
            {
                await OnLongPressCompletedAsync();
                break;
            }

            await Task.Delay(UpdateInterval);
        }
    }

    private async void UnloadProgress()
    {
        if (currentProgress >= MaxProgress) return; // Prevent unloading if progress is already full

        isUnloading = true; // Set unloading flag

        // Smooth unloading when the user releases the button
        while (currentProgress > 0 && !isPressed)
        {
            currentProgress -= ProgressIncrement;
            ProgressOverlay.WidthRequest = (currentProgress / MaxProgress) * ButtonWidth;

            await Task.Delay(UpdateInterval);
        }

        isUnloading = false; // Reset unloading flag after completion
    }

    private void TouchBehavior_CurrentTouchStateChanged(object sender, CommunityToolkit.Maui.Core.TouchStateChangedEventArgs e)
    {
        if (e.State == CommunityToolkit.Maui.Core.TouchState.Pressed)
        {
            // Start or resume filling the progress when pressed
            isPressed = true;
            StartProgress();
        }
        else if (e.State == CommunityToolkit.Maui.Core.TouchState.Default)
        {
            // Stop progress and unload when the touch ends
            isPressed = false;
            UnloadProgress();
        }
    }

    private async Task OnLongPressCompletedAsync()
    {
        await ProgressOverlay.FadeTo(0, 500);  // Fade over 500 milliseconds

        ResetProgress();
    }

    private void ResetProgress()
    {
        // Optionally reset progress when the action is completed, if needed
        currentProgress = 0;
        ProgressOverlay.WidthRequest = 0;
        ProgressOverlay.Opacity = 1;  // Reset the opacity back to fully visible
    }
}
