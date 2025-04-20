# BoysheO.Extensions

simple,high performance, useful extensions
## [6.7.1] - 2025-04-20
Fixed the issue where ThrowIfNull and the original function EnsureNotNull were not synonymous

## [6.7.0] - 2025-04-20 
### What changed

- `EnsureNotNull<T>()` → `ThrowIfNull<T>()`
- `EnsureNotNullOrWhiteSpace()` → `ThrowIfNullOrWhiteSpace()`

### Why

The term *Ensure* can suggest automatic correction or default substitution. To avoid this ambiguity, I’ve renamed these methods to use the more explicit *ThrowIf...* pattern. This makes it immediately clear that these methods throw exceptions when validation fails.

### Impact

Only method names have changed — behavior remains the same. Please update your code accordingly.

## 6.6.1 Updates  
Change

```csharp
void EnsureNotNullOrWhiteSpace(this string? str) 
```

to

```csharp
string EnsureNotNullOrWhiteSpace(this string? str)
```

### 6.6.0 Updates  
New API:IOUtil

### 6.3.3 Updates  
Change EnsureIsNotNullOrWhiteSpace(string) to EnsureNotNullOrWhiteSpace(string).

### 6.3.2 Updates  
Append EnsureIsNotNullOrWhiteSpace(string).I found that it is used very frequently too.

### 6.3.1 Updates  
Append IsNotNullOrWhiteSpace(string).I found that it is used very, very frequently.

### 6.3.0 Updates  
Modify the BoysheO.Extensions.ObjectExtensions.EnsureNotNull method to correct its flawed design.

### 6.2.3(Deprecated) Updates  
Append BoysheO.Extensions.ObjectExtensions.EnsureNotNull method.

### 6.2.2 Updates  
Append [System.Diagnostics.CodeAnalysis.NotNullWhen(false)] to string.IsNullOrEmpty and IsNullOrWhiteSpace

### 6.2.1 Updates  
Support more platform now.  
Optimize performance.  
SpanReaderB.ReadingStringUTF8 was deprecated.  
SpanWriterB.WriteStringUTF8 was deprecated.  
StringUtil.BytesToReadableSize(long) was deprecated.  
TimeUtil.GetCountOfTheTimeBetween was deprecated.

### 6.0.0 Updates  
Remove ArrayPoolUtil which was deprecated.
Remove "global::Extensions" namespace which can cause Unity compile not correct.