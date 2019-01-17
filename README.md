# Schrabber

Schrabber is an application allowing you to download YouTube videos or playlists.  
Additionally you can split those into multiple audio files. For example to extract songs of a compilation as separate files.

Supported as input are:
- YouTube videos
- YouTube playlists
- Local audio/video files

## Assisted Splitting

Splitting into "Parts" can be done via "patterns" or Regex. In case of Regex you would use named capturing groups.

### Example

Matching the songs below:
```
00:00 - Artist 1 - Song 1
02:30 - Artist 2 - Song 2
05:00 - Artist 3 - Song 3
...
1:12:00 - Artist 4 - Song 4
```

Via pattern ``{{Start}} - {{Artist}} - {{Title}}``  
Via Regex ``(?<Start>.+?)\ -\ (?<Author>.+?)\ -\ (?<Title>.+?)\r?$``

### These patterns can also contain literal characters:

```
00:00 「Author」 【Title】
```

Via pattern: ``{{Start}} 「{{Artist}}」 【{{Title}}】``  
Via Regex: ``(?<Start>.+?)\ 「(?<Artist>.+?)」\ 【(?<Title>.+?)】\r?$``

Since those literals are not part of the capturing groups, they won't be in the resulting parts.