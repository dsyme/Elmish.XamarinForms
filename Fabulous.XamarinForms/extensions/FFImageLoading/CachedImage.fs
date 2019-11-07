namespace Fabulous.XamarinForms

open System
open FFImageLoading.Forms
open Fabulous

module ViewAttributes =
    let CachedImageSourceAttribKey = AttributeKey "CachedImage_Source"
    let CachedImageLoadingPlaceholderAttribKey = AttributeKey "CachedImage_LoadingPlaceholder"
    let CachedImageErrorPlaceholderAttribKey = AttributeKey "CachedImage_ErrorPlaceholder"
    let CachedImageCacheTypeAttribKey = AttributeKey "CachedImage_CacheType"
    let CachedImageCacheDurationAttribKey = AttributeKey "CachedImage_CacheDuration"
    let CachedImageCacheKeyFactoryAttribKey = AttributeKey "CachedImage_CacheKeyFactory"
    let CachedImageLoadingDelayAttribKey = AttributeKey "CachedImage_LoadingDelay"
    let CachedImageLoadingPriorityAttribKey = AttributeKey "CachedImage_LoadingPriority"
    let CachedImageCustomDataResolverAttribKey = AttributeKey "CachedImage_CustomDataResolver"
    let CachedImageRetryCountAttribKey = AttributeKey "CachedImage_RetryCount"
    let CachedImageRetryDelayAttribKey = AttributeKey "CachedImage_RetryDelay"
    let CachedImageDownsampleWidthAttribKey = AttributeKey "CachedImage_DownsampleWidth"
    let CachedImageDownsampleHeightAttribKey = AttributeKey "CachedImage_DownsampleHeight"
    let CachedImageDownsampleToViewSizeAttribKey = AttributeKey "CachedImage_DownsampleToViewSize"
    let CachedImageDownsampleUseDipUnitsAttribKey = AttributeKey "CachedImage_DownsampleUseDipUnits"
    let CachedImageFadeAnimationEnabledAttribKey = AttributeKey "CachedImage_FadeAnimationEnabled"
    let CachedImageFadeAnimationDurationAttribKey = AttributeKey "CachedImage_FadeAnimationDuration"
    let CachedImageFadeAnimationForCachedImagesAttribKey = AttributeKey "CachedImage_FadeAnimationForCachedImages"
    let CachedImageBitmapOptimizationsAttribKey = AttributeKey "CachedImage_BitmapOptimizations"
    let CachedImageInvalidateLayoutAfterLoadedAttribKey = AttributeKey "CachedImage_InvalidateLayoutAfterLoaded"
    let CachedImageTransformPlaceholdersAttribKey = AttributeKey "CachedImage_TransformPlaceholders"
    let CachedImageTransformationsAttribKey = AttributeKey "CachedImage_Transformations"
    let CachedImageDownloadStartedAttribKey = AttributeKey "CachedImage_DownloadStarted"
    let CachedImageDownloadProgressAttribKey = AttributeKey "CachedImage_DownloadProgress"
    let CachedImageFileWriteFinishedAttribKey = AttributeKey "CachedImage_FileWriteFinished"
    let CachedImageFinishAttribKey = AttributeKey "CachedImage_Finish"
    let CachedImageSuccessAttribKey = AttributeKey "CachedImage_Success"
    let CachedImageErrorAttribKey = AttributeKey "CachedImage_Error"
open ViewAttributes

[<AutoOpen>]
module FFImageLoadingExtension =
    // Fully-qualified name to avoid extending by mistake
    // another View class (like Xamarin.Forms.View)
    type Fabulous.XamarinForms.View with
        // https://github.com/luberda-molinet/FFImageLoading/wiki/Xamarin.Forms-API
        /// Describes a CachedImage in the view
        // The inline keyword is important for performance
        static member inline CachedImage
            (?source, ?aspect, ?isOpaque, // Align first 3 parameters with Image
             ?loadingPlaceholder, ?errorPlaceholder,
             ?cacheType, ?cacheDuration, ?cacheKeyFactory:ICacheKeyFactory,
             ?loadingDelay, ?loadingPriority,
             ?customDataResolver:FFImageLoading.Work.IDataResolver,
             ?retryCount, ?retryDelay,
             ?downsampleWidth, ?downsampleHeight, ?downsampleToViewSize, ?downsampleUseDipUnits,
             ?fadeAnimationEnabled, ?fadeAnimationDuration, ?fadeAnimationForCachedImages,
             ?bitmapOptimizations, ?invalidateLayoutAfterLoaded,
             ?transformPlaceholders, ?transformations,
             ?downloadStarted, ?downloadProgress, ?fileWriteFinished, ?finish, ?success, ?error,
             // inherited attributes common to all views
             ?gestureRecognizers, ?horizontalOptions, ?margin, ?verticalOptions, ?anchorX, ?anchorY, ?backgroundColor,
             ?behaviors, ?flowDirection, ?height, ?inputTransparent, ?isEnabled, ?isTabStop, ?isVisible, ?minimumHeight,
             ?minimumWidth, ?opacity, ?resources, ?rotation, ?rotationX, ?rotationY, ?scale, ?scaleX, ?scaleY, ?styles,
             ?styleSheets, ?tabIndex, ?translationX, ?translationY, ?visual, ?width, ?style, ?styleClasses, ?shellBackButtonBehavior,
             ?shellBackgroundColor, ?shellDisabledColor, ?shellForegroundColor, ?shellFlyoutBehavior, ?shellNavBarIsVisible,
             ?shellSearchHandler, ?shellTabBarBackgroundColor, ?shellTabBarDisabledColor, ?shellTabBarForegroundColor,
             ?shellTabBarIsVisible, ?shellTabBarTitleColor, ?shellTabBarUnselectedColor, ?shellTitleColor, ?shellTitleView,
             ?shellUnselectedColor, ?automationId, ?classId, ?effects, ?menu, ?ref, ?styleId, ?tag, ?focused, ?unfocused, ?created) =
            
            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match source with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match aspect with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match isOpaque with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingPlaceholder with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match errorPlaceholder with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheType with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheDuration with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheKeyFactory with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingDelay with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingPriority with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match customDataResolver with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match retryCount with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match retryDelay with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleWidth with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleHeight with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleToViewSize with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleUseDipUnits with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationEnabled with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationDuration with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationForCachedImages with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match bitmapOptimizations with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match invalidateLayoutAfterLoaded with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match transformPlaceholders with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match transformations with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downloadStarted with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downloadProgress with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fileWriteFinished with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match finish with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match success with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match error with Some _ -> attribCount + 1 | None -> attribCount
    
            // Populate the attributes of the base element
            let attribs =
                ViewBuilders.BuildView(attribCount, ?gestureRecognizers=gestureRecognizers, ?horizontalOptions=horizontalOptions, ?margin=margin,
                    ?verticalOptions=verticalOptions, ?anchorX=anchorX, ?anchorY=anchorY, ?backgroundColor=backgroundColor, ?behaviors=behaviors,
                    ?flowDirection=flowDirection, ?height=height, ?inputTransparent=inputTransparent, ?isEnabled=isEnabled, ?isTabStop=isTabStop,
                    ?isVisible=isVisible, ?minimumHeight=minimumHeight, ?minimumWidth=minimumWidth, ?opacity=opacity, ?resources=resources,
                    ?rotation=rotation, ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?scaleX=scaleX, ?scaleY=scaleY, ?styles=styles,
                    ?styleSheets=styleSheets, ?tabIndex=tabIndex, ?translationX=translationX, ?translationY=translationY, ?visual=visual, ?width=width,
                    ?style=style, ?styleClasses=styleClasses, ?shellBackButtonBehavior=shellBackButtonBehavior, ?shellBackgroundColor=shellBackgroundColor,
                    ?shellDisabledColor=shellDisabledColor, ?shellForegroundColor=shellForegroundColor, ?shellFlyoutBehavior=shellFlyoutBehavior,
                    ?shellNavBarIsVisible=shellNavBarIsVisible, ?shellSearchHandler=shellSearchHandler, ?shellTabBarBackgroundColor=shellTabBarBackgroundColor,
                    ?shellTabBarDisabledColor=shellTabBarDisabledColor, ?shellTabBarForegroundColor=shellTabBarForegroundColor,
                    ?shellTabBarIsVisible=shellTabBarIsVisible, ?shellTabBarTitleColor=shellTabBarTitleColor, ?shellTabBarUnselectedColor=shellTabBarUnselectedColor,
                    ?shellTitleColor=shellTitleColor, ?shellTitleView=shellTitleView, ?shellUnselectedColor=shellUnselectedColor, ?automationId=automationId,
                    ?classId=classId, ?effects=effects, ?menu=menu, ?ref=ref, ?styleId=styleId, ?tag=tag, ?focused=focused, ?unfocused=unfocused, ?created=created)
                    
            // Add our own attributes. They must have unique names which must match the names below.
            match source with None -> () | Some v -> attribs.Add (CachedImageSourceAttribKey, v)
            match aspect with None -> () | Some v -> attribs.Add (AspectAttribKey, v)
            match isOpaque with None -> () | Some v -> attribs.Add (IsOpaqueAttribKey, v)
            match loadingPlaceholder with None -> () | Some v -> attribs.Add (CachedImageLoadingPlaceholderAttribKey, v)
            match errorPlaceholder with None -> () | Some v -> attribs.Add (CachedImageErrorPlaceholderAttribKey, v)
            match cacheType with None -> () | Some v -> attribs.Add (CachedImageCacheTypeAttribKey, v)
            match cacheDuration with None -> () | Some v -> attribs.Add (CachedImageCacheDurationAttribKey, v)
            match cacheKeyFactory with None -> () | Some v -> attribs.Add (CachedImageCacheKeyFactoryAttribKey, v)
            match loadingDelay with None -> () | Some v -> attribs.Add (CachedImageLoadingDelayAttribKey, v)
            match loadingPriority with None -> () | Some v -> attribs.Add (CachedImageLoadingPriorityAttribKey, v)
            match customDataResolver with None -> () | Some v -> attribs.Add (CachedImageCustomDataResolverAttribKey, v)
            match retryCount with None -> () | Some v -> attribs.Add (CachedImageRetryCountAttribKey, v)
            match retryDelay with None -> () | Some v -> attribs.Add (CachedImageRetryDelayAttribKey, v)
            match downsampleWidth with None -> () | Some v -> attribs.Add (CachedImageDownsampleWidthAttribKey, v)
            match downsampleHeight with None -> () | Some v -> attribs.Add (CachedImageDownsampleHeightAttribKey, v)
            match downsampleToViewSize with None -> () | Some v -> attribs.Add (CachedImageDownsampleToViewSizeAttribKey, v)
            match downsampleUseDipUnits with None -> () | Some v -> attribs.Add (CachedImageDownsampleUseDipUnitsAttribKey, v)
            match fadeAnimationEnabled with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationEnabledAttribKey, v)
            match fadeAnimationDuration with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationDurationAttribKey, v)
            match fadeAnimationForCachedImages with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationForCachedImagesAttribKey, v)
            match bitmapOptimizations with None -> () | Some v -> attribs.Add (CachedImageBitmapOptimizationsAttribKey, v)
            match invalidateLayoutAfterLoaded with None -> () | Some v -> attribs.Add (CachedImageInvalidateLayoutAfterLoadedAttribKey, v)
            match transformPlaceholders with None -> () | Some v -> attribs.Add (CachedImageTransformPlaceholdersAttribKey, v)
            match transformations with None -> () | Some v -> attribs.Add (CachedImageTransformationsAttribKey, v)
            match downloadProgress with None -> () | Some v -> attribs.Add (CachedImageDownloadProgressAttribKey, EventHandler<_>(fun _ -> v))
            match downloadStarted with None -> () | Some v -> attribs.Add (CachedImageDownloadStartedAttribKey, EventHandler<_>(fun _ -> v))
            match fileWriteFinished with None -> () | Some v -> attribs.Add (CachedImageFileWriteFinishedAttribKey, EventHandler<_>(fun _ -> v))
            match finish with None -> () | Some v -> attribs.Add (CachedImageFinishAttribKey, EventHandler<_>(fun _ -> v))
            match success with None -> () | Some v -> attribs.Add (CachedImageSuccessAttribKey, EventHandler<_>(fun _ -> v))
            match error with None -> () | Some v -> attribs.Add (CachedImageErrorAttribKey, EventHandler<_>(fun _ -> v))
    
            // The incremental update method
            let update (prev: ViewElement voption) (curr: ViewElement) (target: CachedImage) =
                ViewBuilders.UpdateView(prev, curr, target)
                curr.UpdatePrimitive (prev, target, CachedImageSourceAttribKey,
                    fun target source -> target.Source <- ViewConverters.convertFabulousImageToXamarinFormsImageSource source)
                curr.UpdatePrimitive (prev, target, AspectAttribKey,
                    fun target aspect -> target.Aspect <- aspect)
                curr.UpdatePrimitive (prev, target, IsOpaqueAttribKey,
                    fun target isOpaque -> target.IsOpaque <- isOpaque)
                curr.UpdatePrimitive (prev, target, CachedImageLoadingPlaceholderAttribKey,
                    fun target loading -> target.LoadingPlaceholder <- ViewConverters.convertFabulousImageToXamarinFormsImageSource loading)
                curr.UpdatePrimitive (prev, target, CachedImageErrorPlaceholderAttribKey,
                    fun target error -> target.ErrorPlaceholder <- ViewConverters.convertFabulousImageToXamarinFormsImageSource error)
                curr.UpdatePrimitive (prev, target, CachedImageCacheTypeAttribKey,
                    fun target cacheType -> target.CacheType <- Option.toNullable cacheType)
                curr.UpdatePrimitive (prev, target, CachedImageCacheDurationAttribKey,
                    fun target cacheDuration -> target.CacheDuration <- Option.toNullable cacheDuration)
                curr.UpdatePrimitive (prev, target, CachedImageCacheKeyFactoryAttribKey,
                    fun target keyFactory -> target.CacheKeyFactory <- keyFactory)
                curr.UpdatePrimitive (prev, target, CachedImageLoadingDelayAttribKey,
                    fun target delay -> target.LoadingDelay <- Option.toNullable delay)
                curr.UpdatePrimitive (prev, target, CachedImageLoadingPriorityAttribKey,
                    fun target priority -> target.LoadingPriority <- priority)
                curr.UpdatePrimitive (prev, target, CachedImageCustomDataResolverAttribKey,
                    fun target resolver -> target.CustomDataResolver <- resolver)
                curr.UpdatePrimitive (prev, target, CachedImageRetryCountAttribKey,
                    fun target retryCount -> target.RetryCount <- retryCount)
                curr.UpdatePrimitive (prev, target, CachedImageRetryDelayAttribKey,
                    fun target delay -> target.RetryDelay <- delay)
                curr.UpdatePrimitive (prev, target, CachedImageDownsampleWidthAttribKey,
                    fun target width -> target.DownsampleWidth <- width)
                curr.UpdatePrimitive (prev, target, CachedImageDownsampleHeightAttribKey,
                    fun target height -> target.DownsampleHeight <- height)
                curr.UpdatePrimitive (prev, target, CachedImageDownsampleToViewSizeAttribKey,
                    fun target downsample -> target.DownsampleToViewSize <- downsample)
                curr.UpdatePrimitive (prev, target, CachedImageDownsampleUseDipUnitsAttribKey,
                    fun target dip -> target.DownsampleUseDipUnits <- dip)
                curr.UpdatePrimitive (prev, target, CachedImageFadeAnimationEnabledAttribKey,
                    fun target fade -> target.FadeAnimationEnabled <- Option.toNullable fade)
                curr.UpdatePrimitive (prev, target, CachedImageFadeAnimationDurationAttribKey,
                    fun target fadeDuration -> target.FadeAnimationDuration <- Option.toNullable fadeDuration)
                curr.UpdatePrimitive (prev, target, CachedImageFadeAnimationForCachedImagesAttribKey,
                    fun target fadeCached -> target.FadeAnimationForCachedImages <- Option.toNullable fadeCached)
                curr.UpdatePrimitive (prev, target, CachedImageBitmapOptimizationsAttribKey,
                    fun target optimize -> target.BitmapOptimizations <- Option.toNullable optimize)
                curr.UpdatePrimitive (prev, target, CachedImageInvalidateLayoutAfterLoadedAttribKey,
                    fun target invalidate -> target.InvalidateLayoutAfterLoaded <- Option.toNullable invalidate)
                curr.UpdatePrimitive (prev, target, CachedImageTransformPlaceholdersAttribKey,
                    fun target transform -> target.TransformPlaceholders <- Option.toNullable transform)
                curr.UpdateElementCollection (prev, CachedImageTransformationsAttribKey, target.Transformations)
                curr.UpdateEvent (prev, CachedImageDownloadStartedAttribKey, target.DownloadStarted)
                curr.UpdateEvent (prev, CachedImageDownloadProgressAttribKey, target.DownloadProgress)
                curr.UpdateEvent (prev, CachedImageFileWriteFinishedAttribKey, target.FileWriteFinished)
                curr.UpdateEvent (prev, CachedImageFinishAttribKey, target.Finish)
                curr.UpdateEvent (prev, CachedImageSuccessAttribKey, target.Success)
                curr.UpdateEvent (prev, CachedImageErrorAttribKey, target.Error)
                
            // Create a ViewElement with the instruction to create and update a CachedImage
            ViewElement.Create(CachedImage, update, attribs)
            
#if DEBUG
    let sample =
        View.CachedImage(
            source = Path "path/to/image.png",
            loadingPlaceholder = Path "path/to/loading-placeholder.png",
            errorPlaceholder = Path "path/to/error-placeholder.png",
            height = 600.,
            width = 600.
        )
#endif