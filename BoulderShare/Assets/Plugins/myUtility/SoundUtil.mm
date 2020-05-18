#import <Foundation/Foundation.h>
#import <AudioToolBox/AudioToolBox.h>

extern "C" void BoulderNotes_Audio(int n)
{
    AudioServicesPlaySystemSound(n);
}