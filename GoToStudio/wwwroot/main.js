import { dotnet } from './_framework/dotnet.js'

const runtime = await dotnet.create();
const config = runtime.getConfig();

await runtime.runMain(config.mainAssemblyName, []);
