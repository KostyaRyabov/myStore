using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using myStore.converters;

namespace myStore.entities
{
    public class Notebook : Accessored<Notebook>, INotifyPropertyChanged, ICloneable
    {
        private int _notebook_id;
        private int? _price;
        private string _name = "";
        private short? _display_max_frequency_hz;
        private decimal? _display_diagonal;

        private string _display_resolution;
        private string _display_technology;
        private string _display_surface;
        private string _display_screen_backlight_type;
        private short? _display_brightness;

        private int? _cpu_id;

        private int? _gpu_id;

        private string _memory_drive_type;
        private string _memory_ram_type;
        private short? _memory_ram_mb;
        private short? _memory_max_ram_gb;
        private short? _memory_amount_of_ram_slots;
        private short? _memory_ssd_capacity_gb;
        private short? _memory_ram_frequency_mhz;
        private short? _memory_emmc_capacity_gb;
        private short? _memory_hdd_capacity_gb;

        private bool? _audio_embedded_speaker;
        private bool? _audio_embedded_microphone;

        private bool? _case_touch_id;
        private string _case_camera;
        private string _case_material;

        private string _keyboard_layout;
        private bool? _keyboard_backlit;

        private short? _interfaces_external_usb32;
        private short? _interfaces_usb_ports;
        private short? _interfaces_usb20;
        private short? _interfaces_usb30;
        private short? _interfaces_usb_type_c;
        private string _interfaces_hdmi_port;
        private short? _interfaces_rj45;
        private string _interfaces_bluetooth;
        private ObservableCollection<string> _interfaces_wifi = new ObservableCollection<string>();
        private string _interfaces_headphone_microphone_jack;
        private ObservableCollection<string> _interfaces_memory_cards = new ObservableCollection<string>();
        private bool? _interfaces_embedded_card_reader;
        private bool? _interfaces_thunderbolt;
        private short? _interfaces_thunderbolt3_usb_type_c;

        private decimal? _battery_browsing_internet_wirelessly_h;
        private decimal? _battery_playing_movies_in_itunes_app_h;
        private string _battery_battery_type;
        private short? _battery_cells_count;
        private decimal? _battery_capacity_wph;
        private short? _battery_capacity_mah;
        private decimal? _battery_voltage;
        private decimal? _battery_work_duration;

        private decimal? _dimensions_height;
        private decimal? _dimensions_width;
        private decimal? _dimensions_thickness;
        private short? _dimensions_weight;

        private string _other_producer_code;
        private int? _other_producer_id;

        private string _other_guarantee_period;

        private int? _software_os_id;

        private byte[] _image;

        
        public int notebook_id
        {
            get
            {
                return _notebook_id;
            }
            set
            {
                _notebook_id = value;
                OnPropertyChanged("notebook_id");
            }
        }
        public int? price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnPropertyChanged("price");
            }
        }
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }
        public short? display_max_frequency_hz
        {
            get
            {
                return _display_max_frequency_hz;
            }
            set
            {
                _display_max_frequency_hz = value;
                OnPropertyChanged("display_max_frequency_hz");
            }
        }
        public decimal? display_diagonal
        {
            get
            {
                return _display_diagonal;
            }
            set
            {
                _display_diagonal = value;
                OnPropertyChanged("display_diagonal");
            }
        }
        public string display_resolution
        {
            get
            {
                return _display_resolution;
            }
            set
            {
                _display_resolution = value;
                OnPropertyChanged("display_resolution");
            }
        }
        public string display_technology
        {
            get
            {
                return _display_technology;
            }
            set
            {
                _display_technology = value;
                OnPropertyChanged("display_technology");
            }
        }
        public string display_surface
        {
            get
            {
                return _display_surface;
            }
            set
            {
                _display_surface = value;
                OnPropertyChanged("display_surface");
            }
        }
        public string display_screen_backlight_type
        {
            get
            {
                return _display_screen_backlight_type;
            }
            set
            {
                _display_screen_backlight_type = value;
                OnPropertyChanged("display_screen_backlight_type");
            }
        }
        public short? display_brightness
        {
            get
            {
                return _display_brightness;
            }
            set
            {
                _display_brightness = value;
                OnPropertyChanged("display_brightness");
            }
        }
        public int? cpu_id
        {
            get
            {
                return _cpu_id;
            }
            set
            {
                _cpu_id = value;
                OnPropertyChanged("cpu_id");
            }
        }
        public int? gpu_id
        {
            get
            {
                return _gpu_id;
            }
            set
            {
                _gpu_id = value;
                OnPropertyChanged("gpu_id");
            }
        }
        public string memory_drive_type
        {
            get
            {
                return _memory_drive_type;
            }
            set
            {
                _memory_drive_type = value;
                OnPropertyChanged("memory_drive_type");
            }
        }
        public string memory_ram_type
        {
            get
            {
                return _memory_ram_type;
            }
            set
            {
                _memory_ram_type = value;
                OnPropertyChanged("memory_ram_type");
            }
        }
        public short? memory_ram_mb
        {
            get
            {
                return _memory_ram_mb;
            }
            set
            {
                _memory_ram_mb = value;
                OnPropertyChanged("memory_ram_mb");
            }
        }
        public short? memory_max_ram_gb
        {
            get
            {
                return _memory_max_ram_gb;
            }
            set
            {
                _memory_max_ram_gb = value;
                OnPropertyChanged("memory_max_ram_gb");
            }
        }
        public short? memory_amount_of_ram_slots
        {
            get
            {
                return _memory_amount_of_ram_slots;
            }
            set
            {
                _memory_amount_of_ram_slots = value;
                OnPropertyChanged("memory_amount_of_ram_slots");
            }
        }
        public short? memory_ssd_capacity_gb
        {
            get
            {
                return _memory_ssd_capacity_gb;
            }
            set
            {
                _memory_ssd_capacity_gb = value;
                OnPropertyChanged("memory_ssd_capacity_gb");
            }
        }
        public short? memory_ram_frequency_mhz
        {
            get
            {
                return _memory_ram_frequency_mhz;
            }
            set
            {
                _memory_ram_frequency_mhz = value;
                OnPropertyChanged("memory_ram_frequency_mhz");
            }
        }
        public short? memory_emmc_capacity_gb
        {
            get
            {
                return _memory_emmc_capacity_gb;
            }
            set
            {
                _memory_emmc_capacity_gb = value;
                OnPropertyChanged("memory_emmc_capacity_gb");
            }
        }
        public short? memory_hdd_capacity_gb
        {
            get
            {
                return _memory_hdd_capacity_gb;
            }
            set
            {
                _memory_hdd_capacity_gb = value;
                OnPropertyChanged("memory_hdd_capacity_gb");
            }
        }
        public bool? audio_embedded_speaker
        {
            get
            {
                return _audio_embedded_speaker;
            }
            set
            {
                _audio_embedded_speaker = value;
                OnPropertyChanged("audio_embedded_speaker");
            }
        }
        public bool? audio_embedded_microphone
        {
            get
            {
                return _audio_embedded_microphone;
            }
            set
            {
                _audio_embedded_microphone = value;
                OnPropertyChanged("audio_embedded_microphone");
            }
        }
        public bool? case_touch_id
        {
            get
            {
                return _case_touch_id;
            }
            set
            {
                _case_touch_id = value;
                OnPropertyChanged("case_touch_id");
            }
        }
        public string case_camera
        {
            get
            {
                return _case_camera;
            }
            set
            {
                _case_camera = value;
                OnPropertyChanged("case_camera");
            }
        }
        public string case_material
        {
            get
            {
                return _case_material;
            }
            set
            {
                _case_material = value;
                OnPropertyChanged("case_material");
            }
        }
        public string keyboard_layout
        {
            get
            {
                return _keyboard_layout;
            }
            set
            {
                _keyboard_layout = value;
                OnPropertyChanged("keyboard_layout");
            }
        }
        public bool? keyboard_backlit
        {
            get
            {
                return _keyboard_backlit;
            }
            set
            {
                _keyboard_backlit = value;
                OnPropertyChanged("keyboard_backlit");
            }
        }
        public short? interfaces_external_usb32
        {
            get
            {
                return _interfaces_external_usb32;
            }
            set
            {
                _interfaces_external_usb32 = value;
                OnPropertyChanged("interfaces_external_usb32");
            }
        }
        public short? interfaces_usb_ports
        {
            get
            {
                return _interfaces_usb_ports;
            }
            set
            {
                _interfaces_usb_ports = value;
                OnPropertyChanged("interfaces_usb_ports");
            }
        }
        public short? interfaces_usb20
        {
            get
            {
                return _interfaces_usb20;
            }
            set
            {
                _interfaces_usb20 = value;
                OnPropertyChanged("interfaces_usb20");
            }
        }
        public short? interfaces_usb30
        {
            get
            {
                return _interfaces_usb30;
            }
            set
            {
                _interfaces_usb30 = value;
                OnPropertyChanged("interfaces_usb30");
            }
        }
        public short? interfaces_usb_type_c
        {
            get
            {
                return _interfaces_usb_type_c;
            }
            set
            {
                _interfaces_usb_type_c = value;
                OnPropertyChanged("interfaces_usb_type_c");
            }
        }
        public string interfaces_hdmi_port
        {
            get
            {
                return _interfaces_hdmi_port;
            }
            set
            {
                _interfaces_hdmi_port = value;
                OnPropertyChanged("interfaces_hdmi_port");
            }
        }
        public short? interfaces_rj45
        {
            get
            {
                return _interfaces_rj45;
            }
            set
            {
                _interfaces_rj45 = value;
                OnPropertyChanged("interfaces_rj45");
            }
        }
        public string interfaces_bluetooth
        {
            get
            {
                return _interfaces_bluetooth;
            }
            set
            {
                _interfaces_bluetooth = value;
                OnPropertyChanged("interfaces_bluetooth");
            }
        }

        public object interfaces_wifi
        {
            get
            {
                return _interfaces_wifi;
            }
            set
            {
                if (value is IEnumerable<string> enumerable){
                    foreach (var el in enumerable)
                    {
                        if (!_interfaces_wifi.Contains(el))
                        {
                            _interfaces_wifi.Add(el);
                        }
                    }
                }
                else if (value is string el)
                {
                    if (!_interfaces_wifi.Contains(el))
                    {
                        _interfaces_wifi.Add(el);
                    }
                }

                OnPropertyChanged("interfaces_wifi");
            }
        }

        public string interfaces_headphone_microphone_jack
        {
            get
            {
                return _interfaces_headphone_microphone_jack;
            }
            set
            {
                _interfaces_headphone_microphone_jack = value;
                OnPropertyChanged("interfaces_headphone_microphone_jack");
            }
        }
        public object interfaces_memory_cards
        {
            get
            {
                return _interfaces_memory_cards;
            }
            set
            {
                if (value is IEnumerable<string> enumerable)
                {
                    foreach (var el in enumerable)
                    {
                        if (!_interfaces_memory_cards.Contains(el))
                        {
                            _interfaces_memory_cards.Add(el);
                        }
                    }
                }
                else if (value is string el)
                {
                    if (!_interfaces_memory_cards.Contains(el))
                    {
                        _interfaces_memory_cards.Add(el);
                    }
                }

                OnPropertyChanged("interfaces_memory_cards");
            }
        }
        public bool? interfaces_embedded_card_reader
        {
            get
            {
                return _interfaces_embedded_card_reader;
            }
            set
            {
                _interfaces_embedded_card_reader = value;
                OnPropertyChanged("interfaces_embedded_card_reader");
            }
        }
        public bool? interfaces_thunderbolt
        {
            get
            {
                return _interfaces_thunderbolt;
            }
            set
            {
                _interfaces_thunderbolt = value;
                OnPropertyChanged("interfaces_thunderbolt");
            }
        }
        public short? interfaces_thunderbolt3_usb_type_c
        {
            get
            {
                return _interfaces_thunderbolt3_usb_type_c;
            }
            set
            {
                _interfaces_thunderbolt3_usb_type_c = value;
                OnPropertyChanged("interfaces_thunderbolt3_usb_type_c");
            }
        }
        public decimal? battery_browsing_internet_wirelessly_h
        {
            get
            {
                return _battery_browsing_internet_wirelessly_h;
            }
            set
            {
                _battery_browsing_internet_wirelessly_h = value;
                OnPropertyChanged("battery_browsing_internet_wirelessly_h");
            }
        }
        public decimal? battery_playing_movies_in_itunes_app_h
        {
            get
            {
                return _battery_playing_movies_in_itunes_app_h;
            }
            set
            {
                _battery_playing_movies_in_itunes_app_h = value;
                OnPropertyChanged("battery_playing_movies_in_itunes_app_h");
            }
        }
        public string battery_battery_type
        {
            get
            {
                return _battery_battery_type;
            }
            set
            {
                _battery_battery_type = value;
                OnPropertyChanged("battery_battery_type");
            }
        }
        public short? battery_cells_count
        {
            get
            {
                return _battery_cells_count;
            }
            set
            {
                _battery_cells_count = value;
                OnPropertyChanged("battery_cells_count");
            }
        }
        public decimal? battery_capacity_wph
        {
            get
            {
                return _battery_capacity_wph;
            }
            set
            {
                _battery_capacity_wph = value;
                OnPropertyChanged("battery_capacity_wph");
            }
        }
        public short? battery_capacity_mah
        {
            get
            {
                return _battery_capacity_mah;
            }
            set
            {
                _battery_capacity_mah = value;
                OnPropertyChanged("battery_capacity_mah");
            }
        }
        public decimal? battery_voltage
        {
            get
            {
                return _battery_voltage;
            }
            set
            {
                _battery_voltage = value;
                OnPropertyChanged("battery_voltage");
            }
        }
        public decimal? battery_work_duration
        {
            get
            {
                return _battery_work_duration;
            }
            set
            {
                _battery_work_duration = value;
                OnPropertyChanged("battery_work_duration");
            }
        }
        public decimal? dimensions_height
        {
            get
            {
                return _dimensions_height;
            }
            set
            {
                _dimensions_height = value;
                OnPropertyChanged("dimensions_height");
            }
        }
        public decimal? dimensions_width
        {
            get
            {
                return _dimensions_width;
            }
            set
            {
                _dimensions_width = value;
                OnPropertyChanged("dimensions_width");
            }
        }
        public decimal? dimensions_thickness
        {
            get
            {
                return _dimensions_thickness;
            }
            set
            {
                _dimensions_thickness = value;
                OnPropertyChanged("dimensions_thickness");
            }
        }
        public short? dimensions_weight
        {
            get
            {
                return _dimensions_weight;
            }
            set
            {
                _dimensions_weight = value;
                OnPropertyChanged("dimensions_weight");
            }
        }
        public string other_producer_code
        {
            get
            {
                return _other_producer_code;
            }
            set
            {
                _other_producer_code = value;
                OnPropertyChanged("other_producer_code");
            }
        }
        public int? other_producer_id
        {
            get
            {
                return _other_producer_id;
            }
            set
            {
                _other_producer_id = value;
                OnPropertyChanged("other_producer_id");
            }
        }
        public string other_guarantee_period
        {
            get
            {
                return _other_guarantee_period;
            }
            set
            {
                _other_guarantee_period = value;
                OnPropertyChanged("other_guarantee_period");
            }
        }
        public int? software_os_id
        {
            get
            {
                return _software_os_id;
            }
            set
            {
                _software_os_id = value;
                OnPropertyChanged("software_os_id");
            }
        }
        public object image
        {
            get
            {
                if (_image is null) return Properties.Resources.unloaded_image;
                else return _image;
            }
            set
            {
                if (value is byte[] byte_array)
                {
                    _image = byte_array;
                }
                else if (value is BitmapSource bitmap_source)
                {
                    _image = ImageConvertor.BmpSource2bArray(bitmap_source);
                }
                else if (value is null)
                {
                    _image = null;
                }

                OnPropertyChanged("image");
            }
        }


        public object Clone()
        {
            Notebook clone = (Notebook)this.MemberwiseClone();

            //clone._image = new List<byte>(this._image).ToArray();

            clone._interfaces_wifi = new ObservableCollection<string>(this._interfaces_wifi);
            clone._interfaces_memory_cards = new ObservableCollection<string>(this._interfaces_memory_cards);

            return clone;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
