using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Log.Networking;
using OpenNos.Log.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenNos.Log.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Members

        public AccountDTO account;
        private bool _orderDesc;

        #endregion

        #region Instantiation

        public MainWindow(AccountDTO accountDTO)
        {
            account = accountDTO;
            InitializeComponent();
            List<LogEntry> logs = LogServiceClient.Instance.GetChatLogEntries(null, null, null, null, null, DateTime.UtcNow.AddMinutes(-15), null, null);
            resultlistbox.ItemsSource = logs;
            typedropdown.Items.Add("All");
            typedropdown.Items.Add(LogType.Map);
            typedropdown.Items.Add(LogType.Speaker);
            typedropdown.Items.Add(LogType.Whisper);
            typedropdown.Items.Add(LogType.Group);
            typedropdown.Items.Add(LogType.Family);
            typedropdown.Items.Add(LogType.BuddyTalk);
            typedropdown.Items.Add(LogType.UserCommand);
            typedropdown.Items.Add(LogType.ModeratorCommand);
            if (account.Authority >= AuthorityType.GM)
            {
                typedropdown.Items.Add(LogType.Trade);
                typedropdown.Items.Add(LogType.BazaarSell);
                typedropdown.Items.Add(LogType.PrivateShopSell);
                typedropdown.Items.Add(LogType.FamilyStorage);
                typedropdown.Items.Add(LogType.Drop);
                typedropdown.Items.Add(LogType.ItemCreate);
            }
            if (account.Authority >= AuthorityType.SGM)
            {
                typedropdown.Items.Add(LogType.GMCommand);
            }
            if (account.Authority >= AuthorityType.GA)
            {
                typedropdown.Items.Add(LogType.MallItemBuy);
                typedropdown.Items.Add(LogType.Packet);
            }
            typedropdown.SelectedIndex = 0;

            ContextMenu rmbMenu = new ContextMenu();

            MenuItem copySender = new MenuItem
            {
                Header = "Copy Sender"
            };
            copySender.Click += CopySenderOnClick;
            rmbMenu.Items.Add(copySender);

            MenuItem copySenderId = new MenuItem
            {
                Header = "Copy SenderId"
            };
            copySenderId.Click += CopySenderIdOnClick;
            rmbMenu.Items.Add(copySenderId);

            MenuItem copyReceiver = new MenuItem
            {
                Header = "Copy Receiver"
            };
            copyReceiver.Click += CopyReceiverOnClick;
            rmbMenu.Items.Add(copyReceiver);

            MenuItem copyReceiverId = new MenuItem
            {
                Header = "Copy ReceiverId"
            };
            copyReceiverId.Click += CopyReceiverIdOnClick;
            rmbMenu.Items.Add(copyReceiverId);

            MenuItem copyMessage = new MenuItem
            {
                Header = "Copy Message"
            };
            copyMessage.Click += CopyMessageOnClick;
            rmbMenu.Items.Add(copyMessage);

            MenuItem copyLogEntry = new MenuItem
            {
                Header = "Copy LogEntry"
            };
            copyLogEntry.Click += CopyLogEntryOnClick;
            rmbMenu.Items.Add(copyLogEntry);

            MenuItem searchBidirectionally = new MenuItem
            {
                Header = "Search Bidirectionally"
            };
            searchBidirectionally.Click += SearchBidirectionallyOnClick;
            rmbMenu.Items.Add(searchBidirectionally);

            resultlistbox.ContextMenu = rmbMenu;
        }

        #endregion

        #region Methods

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void CloseFile(object sender, RoutedEventArgs e)
        {
        }

        private void CopyLogEntryOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is LogEntry entry)
            {
                Clipboard.SetText(entry.ToString());
            }
            else if (resultlistbox.SelectedItem is PacketLogEntry entry2)
            {
                Clipboard.SetText(entry2.ToString());
            }
        }

        private void CopyMessageOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is LogEntry entry)
            {
                Clipboard.SetText(entry.Message);
            }
            else if (resultlistbox.SelectedItem is PacketLogEntry entry2)
            {
                Clipboard.SetText(entry2.Packet);
            }
        }

        private void CopyReceiverIdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is LogEntry entry)
            {
                Clipboard.SetText(entry.ReceiverId.ToString());
            }
        }

        private void CopyReceiverOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is LogEntry entry)
            {
                Clipboard.SetText(entry.Receiver);
            }
        }

        private void CopySenderIdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is LogEntry entry)
            {
                Clipboard.SetText(entry.SenderId.ToString());
            }
            else if (resultlistbox.SelectedItem is PacketLogEntry entry2)
            {
                Clipboard.SetText(entry2.SenderId.ToString());
            }
        }

        private void CopySenderOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is LogEntry entry)
            {
                Clipboard.SetText(entry.Sender);
            }
            else if (resultlistbox.SelectedItem is PacketLogEntry entry2)
            {
                Clipboard.SetText(entry2.Sender);
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
        }

        private void SearchBidirectionallyOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is LogEntry entry)
            {
                if (entry.MessageType == LogType.Whisper || entry.MessageType == LogType.BuddyTalk)
                {
                    IEnumerable<LogEntry> tmp = LogServiceClient.Instance.GetChatLogEntries(
                        new[] { entry.Sender }, null, new[] { entry.Receiver }, null, null, null, null, entry.MessageType)
                        .Concat(LogServiceClient.Instance.GetChatLogEntries(
                        new[] { entry.Receiver }, null, new[] { entry.Sender }, null, null, null, null, entry.MessageType));
                    if (_orderDesc)
                    {
                        resultlistbox.ItemsSource = tmp.OrderByDescending(s => s.Timestamp);
                    }
                    else
                    {
                        resultlistbox.ItemsSource = tmp.OrderBy(s => s.Timestamp);
                    }
                }
                else
                {
                    MessageBox.Show("You can ony search Bidirectionally for Whisper and BuddyTalk messages", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void SearchButton(object sender, RoutedEventArgs e)
        {
            string[] _sender = null;
            string[] _senderid = null;
            string[] _receiver = null;
            string[] _receiverid = null;
            string[] _textfilter = null;
            DateTime? _start = null;
            DateTime? _end = null;
            LogType? _logType = null;

            if (!string.IsNullOrWhiteSpace(senderbox.Text))
            {
                _sender = senderbox.Text.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(senderidbox.Text))// && long.TryParse(senderidbox.Text, out long senderid))
            {
                _senderid = senderidbox.Text.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(receiverbox.Text))
            {
                _receiver = receiverbox.Text.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(receiveridbox.Text))// && long.TryParse(receiveridbox.Text, out long receiverid))
            {
                _receiverid = receiveridbox.Text.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(messagebox.Text))
            {
                _textfilter = messagebox.Text.Split(',');
            }
            _start = datestartpicker.Value;
            _end = dateendpicker.Value;
            if (typedropdown.SelectedIndex != 0)
            {
                _logType = (LogType)typedropdown.SelectedValue;
            }

            if (_logType.HasValue && _logType > LogType.Family)
            {
                IEnumerable<PacketLogEntry> tmp = LogServiceClient.Instance.GetPacketLogEntries(_sender, _senderid, _receiver, _receiverid, _textfilter, _start, _end, _logType, account.Authority);
                if (_orderDesc)
                {
                    resultlistbox.ItemsSource = tmp.OrderByDescending(s => s.Timestamp);
                }
                else
                {
                    resultlistbox.ItemsSource = tmp.OrderBy(s => s.Timestamp);
                }
            }
            else
            {
                IEnumerable<LogEntry> tmp = LogServiceClient.Instance.GetChatLogEntries(_sender, _senderid, _receiver, _receiverid, _textfilter, _start, _end, _logType);
                if (_orderDesc)
                {
                    resultlistbox.ItemsSource = tmp.OrderByDescending(s => s.Timestamp);
                }
                else
                {
                    resultlistbox.ItemsSource = tmp.OrderBy(s => s.Timestamp);
                }
            }
        }

        private void SettingsOrderAscending(object sender, RoutedEventArgs e)
        {
            _orderDesc = false;
            Orderdesc.IsChecked = true;
            Orderasc.IsChecked = false;
        }

        private void SettingsOrderDescending(object sender, RoutedEventArgs e)
        {
            _orderDesc = true;
            Orderdesc.IsChecked = true;
            Orderasc.IsChecked = false;
        }

        #endregion
    }
}