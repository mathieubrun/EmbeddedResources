using System;

namespace TestsContracts
{
    public interface IIntegrationTests
    {
        void Response_must_contain_correct_data();
        void Response_must_have_expires_header();
        void Response_must_have_private_cache_header();
        void Response_must_set_not_modified_header();
    }
}
