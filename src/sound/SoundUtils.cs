using Godot;

namespace BitBuster.sound;

public static class SoundUtils
{

    private static readonly RandomNumberGenerator RandomNumberGenerator = new RandomNumberGenerator();
    
    public static void PlayAtRandomPitch(AudioStreamPlayer2D sound)
    {
        sound.PitchScale = RandomNumberGenerator.RandfRange(0.7f, 1.3f);
        sound.Play();
    }
    
}