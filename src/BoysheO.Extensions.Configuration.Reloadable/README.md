 # BoysheO.Extensions.Configuration.Reloadable

 公开了Reload方法的Microsoft.Extensions.Configuration。  

 ## 使用方法 Usage
 在外部实例化并持有ConfigurationSource对象，
 在配置构建完成后，可以通过调用Reload方法来重新加载配置。
 以下是示例代码：

 ### 示例代码 Example：
 ```csharp
  IConfigurationBuilder builder;
  var configurationSource = new ReloadableJsonConfigurationSource(); // 实例化并持有引用
  builder.Add(configurationSource); 
  builder.Build(); // 配置已构建 
  configurationSource.Reload(json); // 加载json配置 
 ```

 ## English Explanation
 The BoysheO.Extensions.Configuration.Reloadable library exposes a Reload method for Microsoft.Extensions.Configuration.

 ### Usage
 First, instantiate and hold a reference to the ConfigurationSource object.
 After the configuration is built, you can call the Reload method to reload the configuration when needed.
 See the example code.
