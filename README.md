# Audio server
It's audio server for playing audio synchronously in different rooms in my flat. 

It includes 3 parts:

1. [Updated NAudio library](/3rdParty/NAudio) with integrated functionality to play music syncronously on different audio cards on a computer. 
You can use like this one [usb audio card](https://ru.aliexpress.com/item/External-USB-AUDIO-SOUND-CARD-ADAPTER-VIRTUAL-7-1-ch-USB-2-0-Mic-Speaker-Audio/32582886126.html?spm=2114.03010208.3.10.lWZBIf&ws_ab_test=searchweb0_0,searchweb201602_3_10065_10068_10501_10503_10000032_119_10000025_10000029_430_10000028_10060_10062_10056_10055_10000062_10054_10059_10099_10000022_10000012_10103_10000015_10102_10096_10000018_10000019_10000056_10000059_10052_10053_10107_10050_10106_10051_10000053_10000007_10000050_10117_10084_10083_10118_10000047_10080_10082_10081_10110_10111_10112_10113_10114_10115_10000041_10000044_10078_10079_10000038_429_10073_10000035_10121-10503_10501,searchweb201603_9,afswitch_1,single_sort_2_default&btsid=fa87535b-e9fc-467c-b414-52ffe159c1d1)
2. [Audio server](/AudioServer/Alnet.AudioServer). It's concole application that do all work.
 * Has configuration for audio cards
 * Controls playing of audios
 * Provides [WCF endpoint](/AudioServer/Alnet.AudioServerContract) for connecting and manipulating itself
3. [Web site](/AudioServer/Alnet.AudioServer.Web)
  - Light weight web interface for manipulating audio server
