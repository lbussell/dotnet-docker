{{
    set dotnetVersion to join(slice(split(PRODUCT_VERSION, "."), 0, 2), ".") ^
    set isAlpine to find(OS_ARCH_HYPHENATED, "Alpine") >= 0 ^
    set isUbuntu to find(OS_ARCH_HYPHENATED, "Ubuntu") >= 0 ^
    set isMariner to defined(match(OS_VERSION, "^cbl-mariner\d+\.\d+$")) ^
    set baseUrl to VARIABLES[cat("base-url|", dotnetVersion, "|", VARIABLES["branch"])] ^
    set baseImageRepo to when(isAlpine,
        cat(ARCH_VERSIONED, "/alpine"),
        when(isUbuntu,
            "ubuntu.azurecr.io/ubuntu",
            when(isMariner,
                "mcr.microsoft.com/cbl-mariner/base/core",
                "<NOT-IMPLEMENTED>"))) ^
    set baseImageTag to when(isAlpine || isMariner, OS_VERSION_NUMBER, OS_VERSION) ^
    set username to "app" ^
    set uid to 1654 ^
    set gid to uid ^
    set nonRootUserComment to "# Create a non-root user and group"
}}FROM {{baseImageRepo}}:{{baseImageTag}}

{{InsertTemplate("../Dockerfile.common-dotnet-envs", [ "uid": uid ])}}

RUN {{InsertTemplate("../Dockerfile.linux.install-deps")}}

{{nonRootUserComment}}
RUN {{InsertTemplate("../runtime-deps/Dockerfile.linux.non-root-user", [ "name": username ])}}
