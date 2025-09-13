// using Terestrium.Client.Localization;
// using Mirage.Engine.UI.Controls;
// using Mirage.Shared.Data;
// using SFML.System;
//
// namespace Terestrium.Client.UI;
//
// public sealed class CreateCharacterWindow : Window
// {
//     public event Action<CreateCharacterEventArgs>? CreateCharacter;
//     public event Action? Cancel;
//
//     public CreateCharacterWindow(List<JobInfo> jobs) : base(TempStyle.Style)
//     {
//         Size = new Vector2i(300, 200);
//         Text = SR.CreateCharacter;
//
//         var nameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2i(105, 40), Size = new Vector2i(180, 25)};
//
//         Add(new Label(TempStyle.Style) {Position = new Vector2i(20, 40), Size = new Vector2i(75, 25), Text = SR.Name, HorizontalAlignment = HorizontalAlignment.Right});
//         Add(nameTextBox);
//
//         var jobComboBox = new ComboBox(TempStyle.Style) {Position = new Vector2i(105, 70), Size = new Vector2i(180, 25)};
//         foreach (var job in jobs)
//         {
//             jobComboBox.Items.Add(job);
//         }
//
//         Add(new Label(TempStyle.Style) {Position = new Vector2i(20, 70), Size = new Vector2i(75, 25), Text = SR.Job, HorizontalAlignment = HorizontalAlignment.Right});
//
//         var radioButtonMale = new RadioButton(TempStyle.Style) {Position = new Vector2i(105, 100), Size = new Vector2i(50, 25), Text = SR.Male};
//         var radioButtonFemale = new RadioButton(TempStyle.Style) {Position = new Vector2i(165, 100), Size = new Vector2i(65, 25), Text = SR.Female};
//
//         Add(new Label(TempStyle.Style) {Position = new Vector2i(20, 100), Size = new Vector2i(75, 25), Text = SR.Gender, HorizontalAlignment = HorizontalAlignment.Right});
//         Add(radioButtonMale);
//         Add(radioButtonFemale);
//
//         var createButton = new Button(TempStyle.Style)
//         {
//             Position = new Vector2i(115, Size.Y - 40),
//             Size = new Vector2i(80, 25),
//             Text = SR.Create
//         };
//
//         createButton.Click += (_, _) =>
//         {
//             var name = nameTextBox.Text.Trim();
//             if (string.IsNullOrEmpty(name))
//             {
//                 return;
//             }
//
//             var gender = radioButtonMale.Checked ? Gender.Male : Gender.Female;
//
//             if (jobComboBox.SelectedItem is not JobInfo jobInfo)
//             {
//                 return;
//             }
//
//             CreateCharacter?.Invoke(new CreateCharacterEventArgs(name, gender, jobInfo.Id));
//         };
//
//         var cancelButton = new Button(TempStyle.Style)
//         {
//             Position = new Vector2i(205, Size.Y - 40),
//             Size = new Vector2i(80, 25),
//             Text = SR.Cancel
//         };
//
//         cancelButton.Click += (_, _) => Cancel?.Invoke();
//
//         Add(createButton);
//         Add(cancelButton);
//
//         Add(jobComboBox);
//     }
// }