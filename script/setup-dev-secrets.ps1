
$BLOG_LOCAL_ENDPOINT = "http://localhost:5013"
$BLOG_REMOTE_ENDPOINT = "http://localhost:5013"
$API_LOCAL_ENDPOINT = "http://localhost:5011"
$FILE_REMOTE_ENDPOINT = "http://localhost:5013"
$ADMIN_REMOTE_ENDPOINT = "http://localhost:5012"
$JARVIS_REMOTE_ENDPOINT = "http://localhost:5014"
$JARVIS_LOCAL_ENDPOINT = "http://localhost:5014"
$ADMIN_USER_NAME = "test"
$ADMIN_EMAIL = "JerryBian@outlook.com"
$ASSET_LOCATION = "../../sample"
$ADMIN_CHINESE_NAME = "周杰伦"
$ADMIN_ENGLISH_NAME = "Jerry Bian"
$DATA_PROTECTION_KEY_PATH = "../../sample/data_protection"
$HTTP_REQUEST_TOKEN = "token_dev"
$SKIP_GIT_OPERATIONS = "True"

##################
##              ##
##################
$BLOG_PROJECT = Join-Path $PSScriptRoot .. src blog
dotnet user-secrets init --project $BLOG_PROJECT
dotnet user-secrets set "BLOG_LOCAL_ENDPOINT" "$BLOG_LOCAL_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "BLOG_REMOTE_ENDPOINT" "$BLOG_REMOTE_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "API_LOCAL_ENDPOINT" "$API_LOCAL_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_USER_NAME" "$ADMIN_USER_NAME" --project $BLOG_PROJECT
dotnet user-secrets set "FILE_REMOTE_ENDPOINT" "$FILE_REMOTE_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_EMAIL" "$ADMIN_EMAIL" --project $BLOG_PROJECT
dotnet user-secrets set "ASSET_LOCATION" "$ASSET_LOCATION" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_CHINESE_NAME" "$ADMIN_CHINESE_NAME" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_ENGLISH_NAME" "$ADMIN_ENGLISH_NAME" --project $BLOG_PROJECT
dotnet user-secrets set "DATA_PROTECTION_KEY_PATH" "$DATA_PROTECTION_KEY_PATH" --project $BLOG_PROJECT
dotnet user-secrets set "HTTP_REQUEST_TOKEN" "$HTTP_REQUEST_TOKEN" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_REMOTE_ENDPOINT" "$ADMIN_REMOTE_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "JARVIS_REMOTE_ENDPOINT" "$JARVIS_REMOTE_ENDPOINT" --project $BLOG_PROJECT

$POSTS_PER_PAGE = "8"
dotnet user-secrets set "POSTS_PER_PAGE" "$POSTS_PER_PAGE" --project $BLOG_PROJECT

##################
##              ##
##################
$API_PROJECT = Join-Path $PSScriptRoot .. src api
dotnet user-secrets init --project $API_PROJECT
dotnet user-secrets set "BLOG_LOCAL_ENDPOINT" "$BLOG_LOCAL_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "BLOG_REMOTE_ENDPOINT" "$BLOG_REMOTE_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "API_LOCAL_ENDPOINT" "$API_LOCAL_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "ADMIN_USER_NAME" "$ADMIN_USER_NAME" --project $API_PROJECT
dotnet user-secrets set "FILE_REMOTE_ENDPOINT" "$FILE_REMOTE_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "ADMIN_EMAIL" "$ADMIN_EMAIL" --project $API_PROJECT
dotnet user-secrets set "ASSET_LOCATION" "$ASSET_LOCATION" --project $API_PROJECT
dotnet user-secrets set "ADMIN_CHINESE_NAME" "$ADMIN_CHINESE_NAME" --project $API_PROJECT
dotnet user-secrets set "ADMIN_ENGLISH_NAME" "$ADMIN_ENGLISH_NAME" --project $API_PROJECT
dotnet user-secrets set "DATA_PROTECTION_KEY_PATH" "$DATA_PROTECTION_KEY_PATH" --project $API_PROJECT
dotnet user-secrets set "HTTP_REQUEST_TOKEN" "$HTTP_REQUEST_TOKEN" --project $API_PROJECT
dotnet user-secrets set "ADMIN_REMOTE_ENDPOINT" "$ADMIN_REMOTE_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "JARVIS_REMOTE_ENDPOINT" "$JARVIS_REMOTE_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "SKIP_GIT_OPERATIONS" "$SKIP_GIT_OPERATIONS" --project $API_PROJECT
dotnet user-secrets set "JARVIS_LOCAL_ENDPOINT" "$JARVIS_LOCAL_ENDPOINT" --project $API_PROJECT

##################
##              ##
##################
$ADMIN_PROJECT = Join-Path $PSScriptRoot .. src admin
dotnet user-secrets init --project $ADMIN_PROJECT
dotnet user-secrets set "BLOG_LOCAL_ENDPOINT" "$BLOG_LOCAL_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "BLOG_REMOTE_ENDPOINT" "$BLOG_REMOTE_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "API_LOCAL_ENDPOINT" "$API_LOCAL_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_USER_NAME" "$ADMIN_USER_NAME" --project $ADMIN_PROJECT
dotnet user-secrets set "FILE_REMOTE_ENDPOINT" "$FILE_REMOTE_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_EMAIL" "$ADMIN_EMAIL" --project $ADMIN_PROJECT
dotnet user-secrets set "ASSET_LOCATION" "$ASSET_LOCATION" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_CHINESE_NAME" "$ADMIN_CHINESE_NAME" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_ENGLISH_NAME" "$ADMIN_ENGLISH_NAME" --project $ADMIN_PROJECT
dotnet user-secrets set "DATA_PROTECTION_KEY_PATH" "$DATA_PROTECTION_KEY_PATH" --project $ADMIN_PROJECT
dotnet user-secrets set "HTTP_REQUEST_TOKEN" "$HTTP_REQUEST_TOKEN" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_REMOTE_ENDPOINT" "$ADMIN_REMOTE_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "JARVIS_REMOTE_ENDPOINT" "$JARVIS_REMOTE_ENDPOINT" --project $ADMIN_PROJECT

$ADMIN_PASSWORD = "test"
dotnet user-secrets set "ADMIN_PASSWORD" "$ADMIN_PASSWORD" --project $ADMIN_PROJECT

##################
##              ##
##################
$JARVIS_PROJECT = Join-Path $PSScriptRoot .. src jarvis
dotnet user-secrets init --project $JARVIS_PROJECT
dotnet user-secrets set "BLOG_LOCAL_ENDPOINT" "$BLOG_LOCAL_ENDPOINT" --project $JARVIS_PROJECT
dotnet user-secrets set "BLOG_REMOTE_ENDPOINT" "$BLOG_REMOTE_ENDPOINT" --project $JARVIS_PROJECT
dotnet user-secrets set "API_LOCAL_ENDPOINT" "$API_LOCAL_ENDPOINT" --project $JARVIS_PROJECT
dotnet user-secrets set "ADMIN_USER_NAME" "$ADMIN_USER_NAME" --project $JARVIS_PROJECT
dotnet user-secrets set "FILE_REMOTE_ENDPOINT" "$FILE_REMOTE_ENDPOINT" --project $JARVIS_PROJECT
dotnet user-secrets set "ADMIN_EMAIL" "$ADMIN_EMAIL" --project $JARVIS_PROJECT
dotnet user-secrets set "ASSET_LOCATION" "$ASSET_LOCATION" --project $JARVIS_PROJECT
dotnet user-secrets set "ADMIN_CHINESE_NAME" "$ADMIN_CHINESE_NAME" --project $JARVIS_PROJECT
dotnet user-secrets set "ADMIN_ENGLISH_NAME" "$ADMIN_ENGLISH_NAME" --project $JARVIS_PROJECT
dotnet user-secrets set "DATA_PROTECTION_KEY_PATH" "$DATA_PROTECTION_KEY_PATH" --project $JARVIS_PROJECT
dotnet user-secrets set "HTTP_REQUEST_TOKEN" "$HTTP_REQUEST_TOKEN" --project $JARVIS_PROJECT
dotnet user-secrets set "ADMIN_REMOTE_ENDPOINT" "$ADMIN_REMOTE_ENDPOINT" --project $JARVIS_PROJECT
dotnet user-secrets set "JARVIS_REMOTE_ENDPOINT" "$JARVIS_REMOTE_ENDPOINT" --project $JARVIS_PROJECT