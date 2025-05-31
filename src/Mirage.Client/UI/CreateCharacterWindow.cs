using Mirage.Client.Localization;
using Mirage.Engine.UI.Controls;
using Mirage.Shared.Data;
using SFML.System;

namespace Mirage.Client.UI;

public sealed class CreateCharacterWindow : Window
{
    public event Action<CreateCharacterEventArgs>? CreateCharacter;
    public event Action? Cancel;

    public CreateCharacterWindow(List<JobInfo> jobs) : base(TempStyle.Style)
    {
        Width = 300;
        Height = 200;
        Text = SR.CreateCharacter;

        var nameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(105, 40), Width = 180, Height = 25};

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 40), Width = 75, Height = 25, Text = SR.Name, HorizontalAlignment = HorizontalAlignment.Right});
        Add(nameTextBox);

        var jobComboBox = new ComboBox {Position = new Vector2f(105, 70), Width = 180, Height = 25};
        foreach (var job in jobs)
        {
            jobComboBox.Items.Add(job);
        }

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 70), Width = 75, Height = 25, Text = SR.Job, HorizontalAlignment = HorizontalAlignment.Right});

        var radioButtonMale = new RadioButton {Position = new Vector2f(105, 100), Width = 50, Height = 25, Text = SR.Male};
        var radioButtonFemale = new RadioButton {Position = new Vector2f(165, 100), Width = 65, Height = 25, Text = SR.Female};

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 100), Width = 75, Height = 25, Text = SR.Gender, HorizontalAlignment = HorizontalAlignment.Right});
        Add(radioButtonMale);
        Add(radioButtonFemale);

        var createButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(115, Height - 40),
            Width = 80,
            Height = 25,
            Text = SR.Create
        };

        createButton.Click += () =>
        {
            var name = nameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var gender = radioButtonMale.Checked ? Gender.Male : Gender.Female;

            if (jobComboBox.SelectedItem is not JobInfo jobInfo)
            {
                return;
            }

            CreateCharacter?.Invoke(new CreateCharacterEventArgs(name, gender, jobInfo.Id));
        };

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, Height - 40),
            Width = 80,
            Height = 25,
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(createButton);
        Add(cancelButton);

        Add(jobComboBox);
    }
}