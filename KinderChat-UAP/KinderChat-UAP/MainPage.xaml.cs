using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KinderChat.ServerClient.Managers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KinderChat_UAP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ChangeAvatar_OnClick(object sender, RoutedEventArgs e)
        {
            var userManager = new UserManager("HhYZWTPIldWkWtlmIcx8CLAfE4jZacU4yPg-kv99yB5n9sP1mokPtwgGelQyjbRWcrrLpp6vAwtS7fDKN68U1fh_--k3XQ-LgKraK6W2IktCjuLkYqkIA7KB5G5Qv8Ym6YhSo7y62Fo9Uz4nzHatm5EFaDRk8odPYmkr9ksGi9sH5PaCI5_HjY6UKb2fNIpjHYqpYiBNkJ1EnWlnYHDK1JJp4fk3RSWp9ohFpuXjNmKtzRTqomF28OuCfUvME_Lf9mLsP0peTQX4_yGMtFPdjYJ0qx0b4vcRBEQos4Q8hHf0vmG2ZPNB6ARKgw5ZVzuKf1zkQOqaQe76e0VwvHrZ8fF9S6leZE0wfknGwDglpGfyx71qqovDZ_tHgXAsp5oCDqOpbVPZwX5FgrHsKZInOsj4tLakCpjiPKQmYOmD74MzVyHj2ExOukpR9utsY4FrLiBh3NOcaeau9U_ZjLkGDIcKVSX8SRbr9SnHms3HvQ6-zfSe_isudmFHNxAcZ_t0IuTwC_WR4J26N0IsusjvgHiAvBXLh_73cvmcxvJfIbw");
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            await userManager.AddCustomAvatar(stream.AsStreamForRead());
        }

        private async void ChangeNickname_OnClick(object sender, RoutedEventArgs e)
        {
            var userManager = new UserManager("HhYZWTPIldWkWtlmIcx8CLAfE4jZacU4yPg-kv99yB5n9sP1mokPtwgGelQyjbRWcrrLpp6vAwtS7fDKN68U1fh_--k3XQ-LgKraK6W2IktCjuLkYqkIA7KB5G5Qv8Ym6YhSo7y62Fo9Uz4nzHatm5EFaDRk8odPYmkr9ksGi9sH5PaCI5_HjY6UKb2fNIpjHYqpYiBNkJ1EnWlnYHDK1JJp4fk3RSWp9ohFpuXjNmKtzRTqomF28OuCfUvME_Lf9mLsP0peTQX4_yGMtFPdjYJ0qx0b4vcRBEQos4Q8hHf0vmG2ZPNB6ARKgw5ZVzuKf1zkQOqaQe76e0VwvHrZ8fF9S6leZE0wfknGwDglpGfyx71qqovDZ_tHgXAsp5oCDqOpbVPZwX5FgrHsKZInOsj4tLakCpjiPKQmYOmD74MzVyHj2ExOukpR9utsY4FrLiBh3NOcaeau9U_ZjLkGDIcKVSX8SRbr9SnHms3HvQ6-zfSe_isudmFHNxAcZ_t0IuTwC_WR4J26N0IsusjvgHiAvBXLh_73cvmcxvJfIbw");
            var result = await userManager.ChangeNickname("Cherokee Jack");
        }

        private async void GetUser_OnClick(object sender, RoutedEventArgs e)
        {
            var userManager = new UserManager("HhYZWTPIldWkWtlmIcx8CLAfE4jZacU4yPg-kv99yB5n9sP1mokPtwgGelQyjbRWcrrLpp6vAwtS7fDKN68U1fh_--k3XQ-LgKraK6W2IktCjuLkYqkIA7KB5G5Qv8Ym6YhSo7y62Fo9Uz4nzHatm5EFaDRk8odPYmkr9ksGi9sH5PaCI5_HjY6UKb2fNIpjHYqpYiBNkJ1EnWlnYHDK1JJp4fk3RSWp9ohFpuXjNmKtzRTqomF28OuCfUvME_Lf9mLsP0peTQX4_yGMtFPdjYJ0qx0b4vcRBEQos4Q8hHf0vmG2ZPNB6ARKgw5ZVzuKf1zkQOqaQe76e0VwvHrZ8fF9S6leZE0wfknGwDglpGfyx71qqovDZ_tHgXAsp5oCDqOpbVPZwX5FgrHsKZInOsj4tLakCpjiPKQmYOmD74MzVyHj2ExOukpR9utsY4FrLiBh3NOcaeau9U_ZjLkGDIcKVSX8SRbr9SnHms3HvQ6-zfSe_isudmFHNxAcZ_t0IuTwC_WR4J26N0IsusjvgHiAvBXLh_73cvmcxvJfIbw");
            var result = await userManager.GetUser("t_miller@outlook.com");
        }

        private async void ChangeAvatarViaId_OnClick(object sender, RoutedEventArgs e)
        {
            var userManager = new AvatarManager("HhYZWTPIldWkWtlmIcx8CLAfE4jZacU4yPg-kv99yB5n9sP1mokPtwgGelQyjbRWcrrLpp6vAwtS7fDKN68U1fh_--k3XQ-LgKraK6W2IktCjuLkYqkIA7KB5G5Qv8Ym6YhSo7y62Fo9Uz4nzHatm5EFaDRk8odPYmkr9ksGi9sH5PaCI5_HjY6UKb2fNIpjHYqpYiBNkJ1EnWlnYHDK1JJp4fk3RSWp9ohFpuXjNmKtzRTqomF28OuCfUvME_Lf9mLsP0peTQX4_yGMtFPdjYJ0qx0b4vcRBEQos4Q8hHf0vmG2ZPNB6ARKgw5ZVzuKf1zkQOqaQe76e0VwvHrZ8fF9S6leZE0wfknGwDglpGfyx71qqovDZ_tHgXAsp5oCDqOpbVPZwX5FgrHsKZInOsj4tLakCpjiPKQmYOmD74MzVyHj2ExOukpR9utsY4FrLiBh3NOcaeau9U_ZjLkGDIcKVSX8SRbr9SnHms3HvQ6-zfSe_isudmFHNxAcZ_t0IuTwC_WR4J26N0IsusjvgHiAvBXLh_73cvmcxvJfIbw");
            var result = await userManager.GetStaticAvatars();
        }
    }
}
