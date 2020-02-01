# Reflection Vs. LowLevel
## Summary
### Only `ValueType` Fields
|  | Reflection | LowLevel | Note |
|:--| :--: | :--: | :-- |
| Elapsed Time (s) | 51.5 | **1.996** | lower is better |
| Total Memory Used (GB) | 6.19 | **2.87** | lower is better |
### Mixed `ReferenceType` and `ValueType` Fields
#### Reflection without __makeref
|  | Reflection | LowLevel | Note |
|:--| :--: | :--: | :-- |
| Dest Object Type | Class | Struct |  |
| Elapsed Time (s) | 0.9083 | **0.1054** | lower is better |
| Total Memory Used (MB) | 394.88 | **172.93** | lower is better |
#### Reflection with __makeref 
|  | Reflection | LowLevel | Note |
|:--| :--: | :--: | :-- |
| Dest Object Type | **Struct** | Struct |  |
| Elapsed Time (s) | 0.5562 | **0.1054** | lower is better |
| Total Memory Used (MB) | 433.06 | **172.93** | lower is better |
## Details
Original Text (KR): [Blog](https://ekfvoddl3535.blog.me/221783566697)  


### Only `ValueType` Fields
#### Reflection
![img0](https://user-images.githubusercontent.com/42625666/73598874-abe0fa80-4580-11ea-8869-5d65ac039c32.png)
#### LowLevel
![img1](https://user-images.githubusercontent.com/42625666/73598886-d763e500-4580-11ea-8865-0fa86099d8ce.png)
### Mixed `ReferenceType` and `ValueType` Fields
#### Reflection without __makeref
![img2](https://user-images.githubusercontent.com/42625666/73598903-f8c4d100-4580-11ea-8b4e-e22dab2d4705.png)
#### Reflection with __makeref
![img3](https://user-images.githubusercontent.com/42625666/73598920-30337d80-4581-11ea-8dce-d3c53580d3f0.png)
#### LowLevel
![img4](https://user-images.githubusercontent.com/42625666/73598911-0ed29180-4581-11ea-93ef-e73fee4aeeda.png)
