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
