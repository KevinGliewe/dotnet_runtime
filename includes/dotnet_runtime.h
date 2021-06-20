
#ifndef __DOTNET_RUNTIME_H__
#define __DOTNET_RUNTIME_H__

#include <cassert>
#include <string>
#include <hostfxr.h>
#include <coreclr_delegates.h>

#if defined(WINDOWS) || defined(_WIN32) || defined(_WIN64)
    #include <Windows.h>    

    # define __WINDOWS__ 1
    
    #if !defined(DOTNET_RUNTIME_STR)
        # define DOTNET_RUNTIME_STR(s) L ## s
    #endif

    #if !defined(DOTNET_RUNTIME_CHR)
        # define DOTNET_RUNTIME_CHR(c) L ## c
    #endif

    #if !defined(DOTNET_RUNTIME_DIR_SEPARATOR)
        # define DOTNET_RUNTIME_DIR_SEPARATOR DOTNET_RUNTIME_STR("\\")
    #endif

    #if _WIN64
        # define DOTNET_RUNTIME_ENV64BIT
    #else
        # define DOTNET_RUNTIME_ENV32BIT
    #endif
#else
    #include <dlfcn.h>
    #include <limits.h>

    #if !defined(DOTNET_RUNTIME_STR)
        # define DOTNET_RUNTIME_STR(s) s
    #endif

    #if !defined(DOTNET_RUNTIME_CHR)
        # define DOTNET_RUNTIME_CHR(c) c
    #endif

    #if !defined(DOTNET_RUNTIME_DIR_SEPARATOR)
        # define DOTNET_RUNTIME_DIR_SEPARATOR DOTNET_RUNTIME_STR("/")
    #endif

    #if __x86_64__ || __ppc64__
        # define DOTNET_RUNTIME_ENV64BIT
    #else
        # define DOTNET_RUNTIME_ENV32BIT
    #endif
#endif

#if !defined(DOTNET_RUNTIME_VERSION)
    # define DOTNET_RUNTIME_VERSION "5.0.3"
#endif

#if !defined(DOTNET_RUNTIME_ARCH)
    #if defined(DOTNET_RUNTIME_ENV32BIT)
        # define DOTNET_RUNTIME_ARCH DOTNET_RUNTIME_STR("x86")
    #elif defined(DOTNET_RUNTIME_ENV64BIT)
        # define DOTNET_RUNTIME_ARCH DOTNET_RUNTIME_STR("x64")
    #endif
#endif

#if defined(__WINDOWS__)
    # define DOTNET_RUNTIME_PLATFORM_NAME DOTNET_RUNTIME_STR("windows")
    # define DOTNET_RUNTIME_PATH_HOSTFXR_FILE DOTNET_RUNTIME_STR("hostfxr.dll")
#elif defined(__linux__)
    # define DOTNET_RUNTIME_PLATFORM_NAME DOTNET_RUNTIME_STR("linux")
    # define DOTNET_RUNTIME_PATH_HOSTFXR_FILE DOTNET_RUNTIME_STR("libhostfxr.so")
#elif defined(__APPLE__)
    # define DOTNET_RUNTIME_PLATFORM_NAME DOTNET_RUNTIME_STR("macos")
    # define DOTNET_RUNTIME_PATH_HOSTFXR_FILE DOTNET_RUNTIME_STR("libhostfxr.dylib")
#else
    # error Platform not supported
#endif



# define DOTNET_RUNTIME_PATH_HOSTFXR \
    DOTNET_RUNTIME_STR("bin") \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_PLATFORM_NAME \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_ARCH \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_STR("host") \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_STR("fxr") \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_VERSION \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_PATH_HOSTFXR_FILE

# define DOTNET_RUNTIME_PATH_FRAMEWORK \
    DOTNET_RUNTIME_STR("bin") \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_PLATFORM_NAME \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_ARCH \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_STR("shared") \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_STR("Microsoft.NETCore.App") \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_VERSION

# define DOTNET_RUNTIME_PATH_RUNTIMECONFIG \
    DOTNET_RUNTIME_STR(".config") \
    DOTNET_RUNTIME_DIR_SEPARATOR \
    DOTNET_RUNTIME_STR("runtimeconfig.json")

using string_t = std::basic_string<char_t>;

namespace dotnet_runtime
{
    namespace unicode
    {
        // https://stackoverflow.com/a/23920015
        inline int is_surrogate(char16_t uc) { return (uc - 0xd800u) < 2048u; }
        inline int is_high_surrogate(char16_t uc) { return (uc & 0xfffffc00) == 0xd800; }
        inline int is_low_surrogate(char16_t uc) { return (uc & 0xfffffc00) == 0xdc00; }

        inline char32_t surrogate_to_utf32(char16_t high, char16_t low) { 
            return (high << 10) + low - 0x35fdc00; 
        }

        inline void convert_utf16_to_utf32(const char16_t *input, 
                                size_t input_size, 
                                char32_t *output) 
        {
            const char16_t * const end = input + input_size;
            while (input < end) {
                const char16_t uc = *input++;
                if (!is_surrogate(uc)) {
                    *output++ = uc; 
                } else {
                    if (is_high_surrogate(uc) && input < end && is_low_surrogate(*input))
                        *output++ = surrogate_to_utf32(uc, *input++);
                    else
                        *output++ = (char32_t)0xFFFD;
                }
            }
        }
    }



#ifdef __WINDOWS__
    inline void *load_library(string_t path)
    {
        HMODULE h = ::LoadLibraryW(path.c_str());
        assert(h != nullptr);
        return (void*)h;
    }
    inline void *get_export(void *h, const char *name)
    {
        void *f = ::GetProcAddress((HMODULE)h, name);
        assert(f != nullptr);
        return f;
    }
    inline void *get_host_handle()
    {
        HMODULE h = ::GetModuleHandle(NULL);
        assert(h != nullptr);
        return (void*)h;
    }
#else
    inline void *load_library(string_t path)
    {
        void *h = dlopen(path.c_str(), RTLD_LAZY | RTLD_LOCAL);
        assert(h != nullptr);
        return h;
    }
    inline void *get_export(void *h, const char *name)
    {
        void *f = dlsym(h, name);
        assert(f != nullptr);
        return f;
    }
    inline void *get_host_handle()
    {
        void *h = dlopen(NULL, RTLD_LAZY | RTLD_LOCAL);
        assert(h != nullptr);
        return h;
    }
#endif

    struct Runtime
    {
        void* m_pLibraryHandle = nullptr;
        hostfxr_handle m_hHostFxrHandle = nullptr;

        hostfxr_initialize_for_runtime_config_fn init_fptr = nullptr;
        hostfxr_get_runtime_delegate_fn get_delegate_fptr = nullptr;
        hostfxr_close_fn close_fptr = nullptr;

        load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;

        inline Runtime(const string_t a_sHostFxrPath, const string_t a_sRuntimeconfigPath)
        {
            // Load hostfxr library
            this->m_pLibraryHandle = dotnet_runtime::load_library(a_sHostFxrPath);
            assert(this->m_pLibraryHandle);

            // Get library entry points
            this->init_fptr = 
                (hostfxr_initialize_for_runtime_config_fn)dotnet_runtime::get_export(
                    this->m_pLibraryHandle,
                    "hostfxr_initialize_for_runtime_config");
            assert(this->init_fptr);

            this->get_delegate_fptr = 
                (hostfxr_get_runtime_delegate_fn)dotnet_runtime::get_export(
                    this->m_pLibraryHandle, 
                    "hostfxr_get_runtime_delegate");
            assert(this->get_delegate_fptr);

            this->close_fptr = 
                (hostfxr_close_fn)dotnet_runtime::get_export(
                    this->m_pLibraryHandle, 
                    "hostfxr_close");
            assert(this->close_fptr);

            // Init runtime
            int rc = init_fptr(a_sRuntimeconfigPath.c_str(), nullptr, &this->m_hHostFxrHandle);
            assert(rc == 0 && this->m_hHostFxrHandle);


            void *load_assembly_and_get_function_pointer_ = nullptr;

            // Get the load assembly function pointer
            rc = get_delegate_fptr(
                this->m_hHostFxrHandle,
                hdt_load_assembly_and_get_function_pointer,
                &load_assembly_and_get_function_pointer_);
            assert(rc == 0 && load_assembly_and_get_function_pointer_);

            this->load_assembly_and_get_function_pointer = 
                (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer_;
        }

        inline ~Runtime()
        {
            if(this->close_fptr && this->m_hHostFxrHandle)
                this->close_fptr(this->m_hHostFxrHandle);
        }

        inline component_entry_point_fn GetComponentEntrypoint(const string_t a_sDllPath, const string_t a_sType, const string_t a_sMethod)
        {
            component_entry_point_fn entry_point = nullptr;

            int rc = this->load_assembly_and_get_function_pointer(
                a_sDllPath.c_str(),
                a_sType.c_str(),
                a_sMethod.c_str(),
                nullptr /*delegate_type_name*/,
                nullptr,
                (void**)&entry_point);
            assert(rc == 0 && entry_point);

            return entry_point;
        }

        inline void* GetCustomEntrypoint(const string_t a_sDllPath, const string_t a_sType, const string_t a_sMethod)
        {
            void* entry_point = nullptr;

            int rc = this->load_assembly_and_get_function_pointer(
                a_sDllPath.c_str(),
                a_sType.c_str(),
                a_sMethod.c_str(),
                UNMANAGEDCALLERSONLY_METHOD,
                nullptr,
                (void**)&entry_point);
            assert(rc == 0 && entry_point);

            return entry_point;
        }
    };

    struct Library
    {
        Runtime *m_pRuntime = nullptr;
        string_t m_sDllPath;
        string_t m_sLibraryName;

        inline Library(Runtime* a_pRuntime, string_t a_sDllPath, string_t a_sLibraryName)
        {
            this->m_pRuntime = a_pRuntime;
            this->m_sDllPath = a_sDllPath;
            this->m_sLibraryName = DOTNET_RUNTIME_STR(", ") + a_sLibraryName;
        }

        inline component_entry_point_fn GetComponentEntrypoint(const string_t a_sType, const string_t a_sMethod)
        {
            return this->m_pRuntime->GetComponentEntrypoint(
                this->m_sDllPath,
                a_sType + this->m_sLibraryName,
                a_sMethod
            );
        }

        inline void* GetCustomEntrypoint(const string_t a_sType, const string_t a_sMethod)
        {
            return this->m_pRuntime->GetCustomEntrypoint(
                this->m_sDllPath,
                a_sType + this->m_sLibraryName,
                a_sMethod
            );
        }
    };
}

#endif