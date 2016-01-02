module IO

// Taken from https://github.com/albertp007/FSound
let readWavFile (path : string) = 
    use fileStream = new System.IO.FileStream(path, System.IO.FileMode.Open)
    use reader = new System.IO.BinaryReader(fileStream)
    // read chunk id - 4 bytes
    let id = reader.ReadBytes(4)
    if id <> "RIFF"B then failwith "Not a WAV file"
    // ignore next 12 bytes - chunk size: 4, format: 4, subChunkId: 4
    reader.ReadBytes(12) |> ignore
    let sizeChunk1 = System.BitConverter.ToInt32(reader.ReadBytes(4), 0)
    let audioFormat = int (System.BitConverter.ToInt16(reader.ReadBytes(2), 0))
    let numChannels = int (System.BitConverter.ToInt16(reader.ReadBytes(2), 0))
    let samplingRate = System.BitConverter.ToInt32(reader.ReadBytes(4), 0)
    // ignore byte rate: 4, block align: 2
    reader.ReadBytes(6) |> ignore
    let bitsPerSample = 
        int (System.BitConverter.ToInt16(reader.ReadBytes(2), 0))
    // ignore the rest of chunk 1
    reader.ReadBytes(sizeChunk1 - 16) |> ignore
    if int audioFormat <> 1 then 
        // Non-PCM
        reader.ReadBytes(4) |> ignore
        let nExtraBytes = System.BitConverter.ToInt32(reader.ReadBytes(4), 0)
        reader.ReadBytes(nExtraBytes) |> ignore
    let idChunk2 = reader.ReadBytes(4)
    if idChunk2 <> "data"B then 
        failwith "Incorrect format: chunk 2 id not 'data'"
    let nBytes = System.BitConverter.ToInt32(reader.ReadBytes(4), 0)
    let raw = reader.ReadBytes(nBytes)
    raw

