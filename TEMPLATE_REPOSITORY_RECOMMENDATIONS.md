# Template Repository Best Practices & Recommendations

This document outlines best practices for managing dotnet template repositories and recommendations for improving this repository.

## Executive Summary

This template repository has several inconsistencies that can lead to confusion and broken functionality when users generate projects. The main categories of issues are:

1. **Naming Inconsistencies** - Directory names don't match documentation
2. **Environment Variable Inconsistencies** - Variables have different names across templates
3. **File Corruption** - Some Makefiles appear to have merge conflicts
4. **Backup Files** - Backup files were committed to the repository
5. **Missing Documentation** - Referenced docs don't exist

## Issues Identified

### ✅ Fixed Issues

1. **Root .gitignore Missing** ✅
   - Added root-level .gitignore to exclude backup files globally
   
2. **Backup Files Committed** ✅
   - Removed 6 `.backup` files from fullstack-orchestrator directory
   
3. **Naming Inconsistency: ui-template vs web-template** ✅
   - Updated all documentation to use `web-template` (matches actual directory)
   - Fixed install-all.ps1 to reference correct paths
   
4. **Template Short Name Inconsistency** ✅
   - Aligned documentation to use `blazor-enterprise` (matches template.json)

### ⚠️ Critical Issues Remaining

1. **Corrupted Web Template Makefile**
   - **Location**: `templates/web-template/Makefile`
   - **Issue**: File has 334 lines with duplicated/merged content (appears to be merge conflict artifact)
   - **Impact**: Make commands will fail or behave unexpectedly
   - **Recommendation**: Restore from a known good version or rebuild from backend template pattern

2. **Environment Variable Naming Inconsistencies**
   - **Examples**:
     - `JAEGER_UI_PORT` (web-template) vs `JAEGER_WEB_PORT` (fullstack-orchestrator) vs `OTEL_UI_PORT` (backend-template)
     - `MAILHOG_UI_PORT` (web-template) vs `MAILHOG_WEB_PORT` (fullstack-orchestrator)
     - `UI_PORT` (web-template) vs `WEB_PORT` (fullstack-orchestrator)
   - **Impact**: Docker compose files won't find expected variables, services won't start on correct ports
   - **Recommendation**: Standardize on one naming convention across all templates

3. **Incomplete Backend Makefile**
   - **Location**: `templates/backend-template/Makefile`
   - **Issue**: Missing Unix/else section (line 29-31 just says "# (unix section unchanged)")
   - **Impact**: Will fail on Linux/Mac systems
   - **Recommendation**: Complete the Unix section or remove cross-platform support claims

4. **Environment File Corruption**
   - **Location**: `templates/web-template/.env.staging` line 51
   - **Issue**: Merged line: `RABBITMQ_USER=appJAEGER_UI_PORT=16686`
   - **Impact**: Environment variables won't parse correctly
   - **Recommendation**: Fix formatting

5. **Docker Compose Variable Mismatches**
   - **Issue**: Docker compose files reference variables that don't exist in .env files
   - **Example**: `docker-compose.fullstack.yml` uses `${JAEGER_UI_PORT}` but `.env.dev` has `JAEGER_WEB_PORT`
   - **Impact**: Services will use defaults instead of configured ports, potential conflicts

## Best Practices for Template Repositories

### 1. **Consistent Naming Conventions**

**DO:**
```
✅ Choose ONE naming scheme and stick to it everywhere
   - Directory: web-template
   - Template Short Name: blazor-enterprise (in template.json)
   - Documentation: web-template
   - Install Scripts: web-template
```

**DON'T:**
```
❌ Mix names (ui-template in docs, web-template in directory)
❌ Use different short names (enterprise-ui vs blazor-enterprise)
```

### 2. **Environment Variable Consistency**

**DO:**
```yaml
# Choose ONE naming pattern and use it everywhere
# Pattern A: Service-based
JAEGER_UI_PORT=16686
MAILHOG_UI_PORT=8025

# OR Pattern B: Component-based  
OBSERVABILITY_JAEGER_PORT=16686
DEVTOOLS_MAILHOG_PORT=8025
```

**DON'T:**
```yaml
❌ Mix patterns across templates
   JAEGER_UI_PORT in one template
   JAEGER_WEB_PORT in another
   OTEL_UI_PORT in a third
```

### 3. **Version Control Hygiene**

**DO:**
```bash
✅ Use .gitignore at multiple levels:
   - Root .gitignore for repo-wide patterns
   - Template .gitignore for template-specific patterns
   
✅ Exclude backup files:
   *.backup
   *.bak
   *~
   
✅ Review commits before pushing
```

**DON'T:**
```bash
❌ Commit backup files
❌ Commit merge conflict artifacts
❌ Commit IDE-specific files without .gitignore
```

### 4. **Documentation Management**

**DO:**
```markdown
✅ Keep documentation in sync with code
✅ Use relative links that work in the repo
✅ Include examples that actually work
✅ Document what exists, not what you plan to create
```

**DON'T:**
```markdown
❌ Reference non-existent docs (UI-TEMPLATE.md, ORCHESTRATOR.md)
❌ Use absolute GitHub URLs that break in forks
❌ Include outdated examples
```

### 5. **Template Testing**

**DO:**
```bash
✅ Test template generation:
   dotnet new install ./templates/web-template
   dotnet new blazor-enterprise -n TestProject
   cd TestProject && make up-dev
   
✅ Verify cross-platform compatibility if claimed
✅ Test with different parameter combinations
✅ Validate environment files parse correctly
```

**DON'T:**
```bash
❌ Assume templates work without testing
❌ Claim cross-platform support without testing on all platforms
❌ Ignore template parameter validation
```

### 6. **Cross-Template Consistency**

When maintaining multiple coordinated templates:

**DO:**
```
✅ Use a shared configuration schema
✅ Document integration patterns explicitly
✅ Keep variable names identical for shared services
✅ Version templates together
✅ Test templates both independently and together
```

**DON'T:**
```
❌ Let templates drift apart in patterns
❌ Assume coordination without validation
❌ Use different port schemes across templates
```

### 7. **Makefile Best Practices**

**DO:**
```makefile
✅ Keep Makefiles simple and focused
✅ Use consistent target names across templates
✅ Include help targets that document available commands
✅ Properly handle cross-platform scenarios or don't claim support
✅ Test on all supported platforms
```

**DON'T:**
```makefile
❌ Create overly complex Makefiles
❌ Leave merge conflict markers
❌ Have incomplete cross-platform code
❌ Duplicate content within the same file
```

## Recommended Actions for This Repository

### Immediate Priority (Breaks Functionality)

1. **Fix Corrupted Makefile**
   - Restore `templates/web-template/Makefile` from clean version
   - Ensure it matches backend pattern for consistency

2. **Fix Environment Variable Naming**
   - Standardize on ONE naming convention (recommend: `JAEGER_UI_PORT`, `MAILHOG_UI_PORT`)
   - Update all `.env.*` files across all templates
   - Update all `docker-compose*.yml` files to match

3. **Fix Environment File Corruption**
   - Fix merged line in `templates/web-template/.env.staging`

4. **Complete Backend Makefile**
   - Add proper Unix section or remove cross-platform claims

### High Priority (User Experience)

5. **Standardize Docker Compose Files**
   - Ensure variable references match .env files
   - Use consistent default values with `${VAR:-default}` pattern

6. **Add Validation Script**
   - Create script to validate consistency across templates
   - Check for: variable naming, port conflicts, file corruption

7. **Create Missing Documentation**
   - Add basic web-template documentation or remove references
   - Document orchestrator usage or remove references

### Medium Priority (Maintainability)

8. **Add Template Tests**
   - Automated tests that generate projects from templates
   - Validate generated projects build and run
   - Test different parameter combinations

9. **Create Contributing Guidelines**
   - Document the coordination requirements
   - Explain variable naming conventions
   - Provide PR checklist

10. **Add CI/CD Validation**
    - Lint YAML files
    - Validate environment files
    - Test template generation
    - Check for common mistakes (backup files, etc.)

## Validation Checklist

Before committing changes to a template repository:

- [ ] All backup files excluded via .gitignore
- [ ] No merge conflict markers in any files
- [ ] Environment variable names consistent across all templates
- [ ] Docker compose variables match .env files
- [ ] Documentation references match actual files
- [ ] Template tested with `dotnet new install` and generation
- [ ] Generated project tested with `make up-dev` (or equivalent)
- [ ] Cross-platform claims validated on all platforms
- [ ] All referenced documentation files exist
- [ ] Example commands in README actually work

## Tools for Validation

Consider creating these helper scripts:

```bash
# validate-env-vars.sh - Check environment variable consistency
# validate-compose.sh - Ensure compose files reference existing env vars
# validate-docs.sh - Check all documentation links are valid
# test-templates.sh - Generate and test each template
```

## Conclusion

Template repositories require extra care to maintain consistency. The coordination between multiple templates (web, backend, orchestrator) adds complexity. By following these best practices and fixing the identified issues, this repository can provide a much better developer experience.

**Key Takeaway**: Consistency is more important than perfection. Choose one naming convention, one pattern, one approach - and use it everywhere.
