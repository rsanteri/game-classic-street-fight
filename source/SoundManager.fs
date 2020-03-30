module SoundManager

open ResourceManager

type SoundHelper =
    { mutable getRelativePosition: int -> int }

let soundHelper = { getRelativePosition = fun a -> 0 }

let playSound (soundName: string) = (getSound soundName).Play() |> ignore

let playSoundWithProps (soundName: string) =
    let sound = getSound soundName
    sound.Play(1.0f, 0.0f, 0.0f)

///
/// TODO fix this shit
/// Panning seems to require using double sound. When using PAN it plays it on LEFT OR RIGHT
///
let playSpacialSound (soundName: string) (baseVolume: float32) (location: int) =
    let sound = getSound soundName
    let position = soundHelper.getRelativePosition location

    let volume =
        if position < 50 && position > -50 then
            baseVolume
        else
            let relativeDistance = float32 ((float32 (abs position) - 50.0f) / 80.0f)

            baseVolume - (relativeDistance)

    if volume < 0.0f then
        false
    else
        let panValue = float32 position / 100.0f

        let pan: float32 =
            if panValue > 1.0f then 1.0f
            else if panValue < -1.0f then -1.0f
            else panValue

        sound.Play(volume, 0.0f, pan)
        sound.Play
            (volume / 2.0f, 0.0f,
             (if pan < 0.0f then 1.0f else -1.0f))
